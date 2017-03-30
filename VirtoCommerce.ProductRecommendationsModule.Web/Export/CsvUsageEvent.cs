using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvUsageEvent
    {
        public CsvUsageEvent(UsageEvent usageEvent)
        {
            CustomerId = usageEvent.CustomerId;
            ItemId = usageEvent.ItemId;
            EventType = usageEvent.EventType.ToString();
            CreatedDate = usageEvent.CreatedDate.ToString("s");
        }

        public string CustomerId { get; set; }

        public string ItemId { get; set; }

        public string EventType { get; set; }

        public string CreatedDate { get; set; }
    }
}