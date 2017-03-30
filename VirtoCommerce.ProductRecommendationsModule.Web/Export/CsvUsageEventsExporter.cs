using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvUsageEventsExporter : CsvExporter
    {
        private readonly IUsageEventService _usageEventService;

        public CsvUsageEventsExporter(IUsageEventService usageEventService)
        {
            _usageEventService = usageEventService;
        }

        public void DoUsageEventsExport(Stream outStream, string fileName, Store store, Action<ExportImportProgressInfo> progressCallback)
        {
            DoExport(outStream, fileName, store.Settings.GetSettingValue("Recommendations.UsageEvents.ChunkSize", 200) * 1024 * 1024,
                "events", () => LoadEvents(store).Select(x => new CsvUsageEvent(x)).ToArray(), new CsvUsageEventMap(), progressCallback);
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