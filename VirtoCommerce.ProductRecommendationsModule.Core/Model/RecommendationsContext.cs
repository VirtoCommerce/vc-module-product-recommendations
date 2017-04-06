using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Model
{
    public class RecommendationsEvaluationContext : IEvaluationContext
    {
        public string StoreId { get; set; }

        public string Type { get; set; }

        public string ModelId { get; set; }

        public string BuildId { get; set; }

        public string UserId { get; set; }

        public string[] ProductsIds { get; set; }

        public int Take { get; set; }
    }
}