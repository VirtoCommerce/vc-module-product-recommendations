using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Model.Search
{
    public class UsageEventSearchCriteria: SearchCriteriaBase
    {
        public string StoreId { get; set; }
    }
}
