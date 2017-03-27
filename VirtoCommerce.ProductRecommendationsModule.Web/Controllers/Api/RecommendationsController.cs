using System;
using System.IO;
using System.IO.Packaging;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Hangfire;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;
using VirtoCommerce.ProductRecommendationsModule.Data.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Export;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Controllers.Api
{
    [RoutePrefix("api/recommendations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RecommendationsController: ApiController
    {
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly ICatalogService _catalogService;
        private readonly IStoreService _storeService;
        private readonly CsvExporter _csvExporter;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IUserEventService _userEventService;
        private readonly ISettingsManager _settingsManager;

        private const int DefaultStreamBufferSize = 80;

        public RecommendationsController(IPushNotificationManager pushNotifier,
            IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver,
            ICatalogService catalogService, IStoreService storeService, CsvExporter csvExporter,
            IUserNameResolver userNameResolver, IUserEventService userEventService,
            ISettingsManager settingsManager)
        {
            _pushNotifier = pushNotifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
            _catalogService = catalogService;
            _storeService = storeService;
            _csvExporter = csvExporter;
            _userNameResolver = userNameResolver;
            _userEventService = userEventService;
            _settingsManager = settingsManager;
        }
        
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(string[]))]
        public IHttpActionResult Get()
        {
            var result = new string[] {
                "9cbd8f316e254a679ba34a900fccb076",
                "f9330eb5ed78427abb4dc4089bc37d9f",
                "d154d30d76d548fb8505f5124d18c1f3",
                "1486f5a1a25f48a999189c081792a379",
                "ad4ae79ffdbc4c97959139a0c387c72e"
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("events")]
        [ResponseType(typeof(void))]
        public IHttpActionResult AddEvent(UserEvent userEvent)
        {
            _userEventService.Add(userEvent);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("catalogs/{catalogId}/export")]
        [ResponseType(typeof(ExportPushNotification))]
        public IHttpActionResult CatalogExport(string catalogId)
        {
            return DoExport("CatalogPrepatedForRecommendationsCsvExport", "Catalog prepared for recommendations export task", notification => BackgroundJob.Enqueue(() => CatalogExportBackground(catalogId, notification)));
        }

        [HttpGet]
        [Route("stores/{storeId}/export")]
        [ResponseType(typeof(ExportPushNotification))]
        public IHttpActionResult UserEventsExport(string storeId)
        {
            return DoExport("UserEventsRelatedToStoreCsvExport", "User events related to store export task", notification => BackgroundJob.Enqueue(() => UserEventsExportBackground(storeId, notification)));
        }

        private IHttpActionResult DoExport(string notifyType, string notificationDescription, Action<ExportPushNotification> job)
        {
            var notification = new ExportPushNotification(_userNameResolver.GetCurrentUserName(), notifyType)
            {
                Title = notificationDescription,
                Description = "Starting export..."
            };
            _pushNotifier.Upsert(notification);

            job(notification);

            return Ok(notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void CatalogExportBackground(string catalogId, ExportPushNotification notification)
        {
            var catalog = _catalogService.GetById(catalogId);
            if (catalog == null)
            {
                throw new NullReferenceException("catalog");
            }

            ExportBackground((stream, progressCallback) => _csvExporter.DoCatalogExport(stream, catalogId, progressCallback),
                catalog.Name,
                _settingsManager.GetValue("ProductRecommendations.Catalog.ChunkSize", DefaultStreamBufferSize) * 1024,
                notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void UserEventsExportBackground(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }

            ExportBackground((stream, progressCallback) => _csvExporter.DoUserEventsExport(stream, storeId, progressCallback),
                store.Name,
                _settingsManager.GetValue("ProductRecommendations.UserEvents.ChunkSize", DefaultStreamBufferSize) * 1024,
                notification);
        }

        private void ExportBackground(Action<Stream, Action<ExportImportProgressInfo>> exporter, string entityName, int chunkSize, ExportPushNotification notification)
        {

            Action<ExportImportProgressInfo> progressCallback = x =>
            {
                notification.TotalCount = x.TotalCount;
                notification.ProcessedCount = x.ProcessedCount;
                notification.Errors = x.Errors;
                _pushNotifier.Upsert(notification);
            };

            using (var stream = new MemoryStream())
            {
                try
                {
                    exporter(stream, progressCallback);

                    var relativeUrl = "temp/" + entityName + ".zip";
                    //Upload result csv to blob storage
                    using (var blobStream = _blobStorageProvider.OpenWrite(relativeUrl))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var package = ZipPackage.Open(blobStream, FileMode.Create))
                        {
                            var i = 0;
                            while (stream.Position < stream.Length)
                            {
                                // ZipPackage uses Uri for file names and Uri don't handle whitespaces as whitespace, it handle it as %20
                                var part = package.CreatePart(PackUriHelper.CreatePartUri(new Uri(entityName.Replace(' ', '_') + "-" + i + ".csv", UriKind.Relative)), "text/csv", CompressionOption.Normal);
                                using (var partStream = part.GetStream())
                                {
                                    stream.CopyTo(partStream, chunkSize);
                                }
                                i++;
                            }
                        }
                    }
                    //Get a download url
                    notification.DownloadUrl = _blobUrlResolver.GetAbsoluteUrl(relativeUrl);
                }
                catch (Exception ex)
                {
                    notification.Description = "Export failed";
                    notification.Errors.Add(ex.ExpandExceptionMessage());
                }
                finally
                {
                    notification.Description = "Export finished";
                    notification.Finished = DateTime.UtcNow;
                    _pushNotifier.Upsert(notification);
                }
            }
        }
    }
}