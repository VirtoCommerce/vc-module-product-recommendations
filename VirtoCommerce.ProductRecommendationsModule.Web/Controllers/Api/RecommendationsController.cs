using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hangfire;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Export;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Controllers.Api
{
    [RoutePrefix("api/recommendations")]
    public class RecommendationsController: ApiController
    {
        private readonly IRecommendationsService _recommendationsService;
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly ICatalogService _catalogService;
        private readonly IStoreService _storeService;
        private readonly CsvExporter _csvExporter;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IUsageEventService _usageEventService;

        public RecommendationsController(IRecommendationsService recommendationsService,
            IStoreService storeService, ICatalogService catalogService,
            IUsageEventService usageEventService, CsvExporter csvExporter,
            IUserNameResolver userNameResolver, IPushNotificationManager pushNotifier,
            IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver)
        {
            _recommendationsService = recommendationsService;
            _pushNotifier = pushNotifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
            _catalogService = catalogService;
            _storeService = storeService;
            _csvExporter = csvExporter;
            _userNameResolver = userNameResolver;
            _usageEventService = usageEventService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(string[]))]
        public async Task<IHttpActionResult> GetCustomerRecommendations(string storeId, string customerId, int numberOfResults)
        {
            var result = await _recommendationsService.GetCustomerRecommendationsAsync(storeId, customerId, numberOfResults);
            return Ok(result);
        }

        [HttpPost]
        [Route("events")]
        [ResponseType(typeof(void))]
        public IHttpActionResult AddEvent(UsageEvent[] usageEvents)
        {
            _usageEventService.Add(usageEvents);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("catalog/export")]
        [ResponseType(typeof(ExportPushNotification))]
        public IHttpActionResult CatalogExport(string storeId)
        {
            return DoExport("CatalogPrepatedForRecommendationsExport", "Catalog prepared for recommendations export task", notification => BackgroundJob.Enqueue(() => CatalogExportBackground(storeId, notification)));
        }

        [HttpGet]
        [Route("events")]
        [ResponseType(typeof(ExportPushNotification))]
        public IHttpActionResult UsageEventsExport(string storeId)
        {
            return DoExport("UsageEventsRelatedToStoreExport", "Usage events related to store export task", notification => BackgroundJob.Enqueue(() => UsageEventsExportBackground(storeId, notification)));
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
        public void CatalogExportBackground(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }
            var catalog = _catalogService.GetById(store.Catalog);
            if (catalog == null)
            {
                throw new NullReferenceException("catalog");
            }

            ExportBackground((stream, progressCallback) => _csvExporter.DoCatalogExport(stream, store, catalog, progressCallback),
                catalog.Name,
                int.Parse(store.Settings.First(x => x.Name == "Recommendations.Catalog.ChunkSize").Value) * 1024,
                notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void UsageEventsExportBackground(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }

            ExportBackground((stream, progressCallback) => _csvExporter.DoUsageEventsExport(stream, store, progressCallback),
                store.Name + " Events",
                int.Parse(store.Settings.First(x => x.Name == "Recommendations.UsageEvents.ChunkSize").Value) * 1024,
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

            using (var exportStream = new MemoryStream())
            {
                try
                {
                    exporter(exportStream, progressCallback);
                    
                    // Split export file to parts with maximum support size
                    var relativeUrl = "temp/" + entityName + ".zip";
                    using (var zipStream = new MemoryStream())
                    {
                        exportStream.Seek(0, SeekOrigin.Begin);
                        using (var package = ZipPackage.Open(zipStream, FileMode.Create))
                        {
                            var i = 0;
                            do
                            {
                                // ZipPackage uses Uri for file names and Uri don't handle whitespaces as whitespace, it handle it as %20
                                var part = package.CreatePart(PackUriHelper.CreatePartUri(
                                    new Uri(entityName.Replace(' ', '_') + "-" + i + ".csv", UriKind.Relative)), "text/csv", CompressionOption.Normal);
                                using (var partStream = part.GetStream())
                                {
                                    exportStream.CopyTo(partStream, chunkSize);
                                }
                                i++;
                            } while (exportStream.Position < exportStream.Length);
                        }

                        // Upload result to blob storage
                        using (var blobStream = _blobStorageProvider.OpenWrite(relativeUrl))
                        {
                            zipStream.Seek(0, SeekOrigin.Begin);
                            zipStream.CopyTo(blobStream);
                        }
                    }
                    // Get a download url
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