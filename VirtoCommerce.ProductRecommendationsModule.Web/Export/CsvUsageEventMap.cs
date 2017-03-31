using System;
using CsvHelper.Configuration;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    [CLSCompliant(false)]
    public class CsvUsageEventMap : CsvClassMap<CsvUsageEvent>
    {
        public CsvUsageEventMap()
        {
            Map(x => x.CustomerId).Index(0);
            Map(x => x.ItemId).Index(1);
            Map(x => x.CreatedDate).Index(2);
            Map(x => x.EventType).Index(3);
        }
    }
}