using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Model
{
    public class UsageEvent : Entity
    {
        public UsageEvent()
        {
            Created = DateTime.UtcNow;
        }
        
        public string CustomerId { get; set; }
        
        public string StoreId { get; set; }
        
        public string ItemId { get; set; }
        
        public UsageEventType EventType { get; set; }
        
        public DateTime Created { get; set; }
    }
}
