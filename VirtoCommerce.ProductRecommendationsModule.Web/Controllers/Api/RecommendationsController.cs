using System;
using System.IO;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Hangfire;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
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
        private readonly CsvCatalogExporter _csvExporter;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IUserEventService _userEventService;

        public RecommendationsController(IPushNotificationManager pushNotifier,
            IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver,
            ICatalogService catalogService, CsvCatalogExporter csvExporter, IUserNameResolver userNameResolver, IUserEventService userEventService)
        {
            _pushNotifier = pushNotifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
            _catalogService = catalogService;
            _csvExporter = csvExporter;
            _userNameResolver = userNameResolver;
            _userEventService = userEventService;
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
            _userEventService.AddEvent(userEvent);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("catalogs/{catalogid}/export")]
        [ResponseType(typeof(CatalogExportPushNotification))]
        public IHttpActionResult CatalogExport(string catalogId)
        {
            var notification = new CatalogExportPushNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Catalog export prepared for recommendations task",
                Description = "Starting export..."
            };
            _pushNotifier.Upsert(notification);

            BackgroundJob.Enqueue(() => CatalogExportBackground(catalogId, notification));

            return Ok(notification);
        }

        private void CatalogExportBackground(string catalogId, CatalogExportPushNotification notification)
        {
            var catalog = _catalogService.GetById(catalogId);
            if (catalog == null)
            {
                throw new NullReferenceException("catalog");
            }

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
                    _csvExporter.DoExport(stream, catalogId, progressCallback);

                    stream.Position = 0;
                    var blobRelativeUrl = "temp/Catalog-" + catalog.Name + "-prepared-for-recommendations-export.csv";
                    //Upload result csv to blob storage
                    using (var blobStream = _blobStorageProvider.OpenWrite(blobRelativeUrl))
                    {
                        stream.CopyTo(blobStream);
                    }
                    //Get a download url
                    notification.DownloadUrl = _blobUrlResolver.GetAbsoluteUrl(blobRelativeUrl);
                    notification.Description = "Export finished";
                }
                catch (Exception ex)
                {
                    notification.Description = "Export failed";
                    // TODO: Replace with ex.ExpandExceptionMessage() if we will use VirtoCommerce.Platform.Data
                    notification.Errors.Add(ex.ToString());
                }
                finally
                {
                    notification.Finished = DateTime.UtcNow;
                    _pushNotifier.Upsert(notification);
                }
            }
        }
    }
}