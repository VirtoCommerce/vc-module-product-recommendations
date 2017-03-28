using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class UsageEventEntity : Entity
    {
        [Required]
        [StringLength(64)]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(64)]
        public string StoreId { get; set; }

        [Required]
        [StringLength(64)]
        public string ItemId { get; set; }

        [Required]
        [StringLength(64)]
        public string EventType { get; set; }
        
        public DateTime Created { get; set; }

        public virtual UsageEvent ToModel(UsageEvent usageEvent)
        {
            if (usageEvent == null)
                throw new ArgumentNullException("usageEvent");

            usageEvent.Id = Id;
            usageEvent.CustomerId = CustomerId;
            usageEvent.StoreId = StoreId;
            usageEvent.ItemId = ItemId;
            usageEvent.EventType = (UsageEventType)Enum.Parse(typeof(UsageEventType), EventType);
            usageEvent.Created = Created;

            return usageEvent;
        }

        public virtual UsageEventEntity FromModel(UsageEvent usageEvent, PrimaryKeyResolvingMap pkMap)
        {
            if (usageEvent == null)
                throw new ArgumentNullException("usageEvent");
            
            pkMap.AddPair(usageEvent, this);

            Id = usageEvent.Id;
            CustomerId = usageEvent.CustomerId;
            StoreId = usageEvent.StoreId;
            ItemId = usageEvent.ItemId;
            EventType = usageEvent.EventType.ToString();
            Created = usageEvent.Created;

            return this;
        }
    }
}
