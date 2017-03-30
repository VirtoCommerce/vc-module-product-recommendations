using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class UsageEventEntity : AuditableEntity
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

        public virtual UsageEvent ToModel(UsageEvent usageEvent)
        {
            if (usageEvent == null)
                throw new ArgumentNullException("usageEvent");

            usageEvent.Id = Id;
            usageEvent.CreatedBy = CreatedBy;
            usageEvent.CreatedDate = CreatedDate;
            usageEvent.ModifiedBy = ModifiedBy;
            usageEvent.ModifiedDate = ModifiedDate;
            usageEvent.CustomerId = CustomerId;
            usageEvent.StoreId = StoreId;
            usageEvent.ItemId = ItemId;
            usageEvent.EventType = (UsageEventType)Enum.Parse(typeof(UsageEventType), EventType);

            return usageEvent;
        }

        public virtual UsageEventEntity FromModel(UsageEvent usageEvent, PrimaryKeyResolvingMap pkMap)
        {
            if (usageEvent == null)
                throw new ArgumentNullException("usageEvent");
            
            pkMap.AddPair(usageEvent, this);

            Id = usageEvent.Id;
            CreatedBy = usageEvent.CreatedBy;
            CreatedDate = usageEvent.CreatedDate;
            ModifiedBy = usageEvent.ModifiedBy;
            ModifiedDate = usageEvent.ModifiedDate;
            CustomerId = usageEvent.CustomerId;
            StoreId = usageEvent.StoreId;
            ItemId = usageEvent.ItemId;
            EventType = usageEvent.EventType.ToString();

            return this;
        }
    }
}
