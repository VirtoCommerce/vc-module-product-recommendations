using System.Collections.Generic;
using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Model
{
    public class RecommendationEvalContext : IEvaluationContext
    {
        public RecommendationEvalContext()
        {
            ProductIds = new List<string>();
        }

        public string Type { get; set; }

        public string StoreId { get; set; }

        public string UserId { get; set; }

        public ICollection<string> ProductIds { get; set; }

        public int Take { get; set; }

        public string ModelId { get; set; }
        public string BuildId { get; set; }
    } 
}