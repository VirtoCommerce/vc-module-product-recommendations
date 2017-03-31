namespace VirtoCommerce.ProductRecommendationsModule.Web.Model
{
    public class CustomerRecommendationsContext
    {
        public string StoreId { get; set; }

        public string CustomerId { get; set; }

        public string[] ProductIds { get; set; }

        public int NumberOfResults { get; set; }
    }
}