using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class UserEvent : Entity
    {
        public UserEvent()
        {
            Created = DateTime.UtcNow;
        }

        public string UserId { get; set; }

        public string ItemId { get; set; }

        public string EventType { get; set; }

        public DateTime Created { get; set; }
    }
}
