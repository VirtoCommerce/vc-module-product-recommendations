using System;
using CsvHelper.Configuration;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    [CLSCompliant(false)]
    public class CsvUsageEventMap : CsvClassMap<UsageEvent>
    {
        public CsvUsageEventMap()
        {
            // https://westus.dev.cognitive.microsoft.com/docs/services/Recommendations.V4.0/operations/56f316efeda5650db055a3e2
            // User Id
            Map(x => x.CustomerId).Index(0);
            // Item Id
            Map(x => x.ItemId).Index(1);
            // Time
            Map(x => x.Created).Index(2);
            // Event
            Map(x => x.EventType).Index(3);
        }
    }
}