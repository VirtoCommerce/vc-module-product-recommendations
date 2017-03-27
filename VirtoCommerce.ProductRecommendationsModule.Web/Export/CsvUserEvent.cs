using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvUserEvent
    {
        public CsvUserEvent(UserEvent userEvent)
        {
            UserId = userEvent.UserId;
            ItemId = userEvent.ItemId;
            EventType = userEvent.EventType.ToString();
            Created = userEvent.Created.ToString("s");
        }

        public string UserId { get; set; }

        public string ItemId { get; set; }

        public string EventType { get; set; }

        public string Created { get; set; }
    }
}