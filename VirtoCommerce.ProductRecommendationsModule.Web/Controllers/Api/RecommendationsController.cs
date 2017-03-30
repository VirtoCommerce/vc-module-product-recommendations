﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hangfire;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
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
        private readonly Exporter _exporter;
        private readonly IStoreService _storeService;
        private readonly ICatalogService _catalogService;
        private readonly CsvCatalogExporter _csvCatalogExporter;
        private readonly IUsageEventService _usageEventService;
        private readonly CsvUsageEventsExporter _csvUsageEventsExporter;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _pushNotifier;

        public RecommendationsController(IRecommendationsService recommendationsService, Exporter exporter,
            IStoreService storeService, ICatalogService catalogService, CsvCatalogExporter csvCatalogExporter,
            IUsageEventService usageEventService, CsvUsageEventsExporter csvUsageEventsExporter,
            IUserNameResolver userNameResolver, IPushNotificationManager pushNotifier)
        {
            _recommendationsService = recommendationsService;
            _exporter = exporter;
            _storeService = storeService;
            _catalogService = catalogService;
            _csvCatalogExporter = csvCatalogExporter;
            _usageEventService = usageEventService;
            _csvUsageEventsExporter = csvUsageEventsExporter;
            _userNameResolver = userNameResolver;
            _pushNotifier = pushNotifier;
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
        [Route("events/export")]
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

            _exporter.Export((stream, fileName, progressCallback) => _csvCatalogExporter.DoCatalogExport(stream, fileName, store, catalog, progressCallback), catalog.Name, notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void UsageEventsExportBackground(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }

            _exporter.Export((stream, fileName, progressCallback) => _csvUsageEventsExporter.DoUsageEventsExport(stream, fileName, store, progressCallback), store.Name + " Events", notification);
        }
    }
}