using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvUsageEventsExporter : CsvExporter
    {
        private readonly IStoreService _storeService;
        private readonly IUsageEventService _usageEventService;

        public CsvUsageEventsExporter(IStoreService storeService, IUsageEventService usageEventService,
            IPushNotificationManager pushNotifier, IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver) : base(pushNotifier, blobStorageProvider, blobUrlResolver)
        {
            _storeService = storeService;
            _usageEventService = usageEventService;
        }

        public void DoUsageEventsExport(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }

            DoExport(store.Name + " Events", store.Settings.GetSettingValue("Recommendations.UsageEvents.ChunkSize", 200) * 1024 * 1024,
                "events", () => LoadEvents(store).Select(x => new CsvUsageEvent(x)).ToArray(), new CsvUsageEventMap(), notification);
        }

        private ICollection<UsageEvent> LoadEvents(Store store)
        {
            var maximumNumberOfEvents = store.Settings.GetSettingValue("Recommendations.UsageEvents.MaximumNumber", int.MaxValue);
            return _usageEventService.Search(new UsageEventSearchCriteria
            {
                StoreId = store.Id,
                Sort = "created:desc",
                Skip = 0,
                Take = maximumNumberOfEvents
            }).Results;
        }
    }
}