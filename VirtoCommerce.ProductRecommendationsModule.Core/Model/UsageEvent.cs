using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Model
{
    public class UsageEvent : AuditableEntity
    {
        public string CustomerId { get; set; }
        
        public string StoreId { get; set; }
        
        public string ItemId { get; set; }
        
        public UsageEventType EventType { get; set; }
    }
}
