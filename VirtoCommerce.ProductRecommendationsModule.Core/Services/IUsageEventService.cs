using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Services
{
    public interface IUsageEventService
    {
        void Add(UsageEvent[] usageEvents);

        GenericSearchResult<UsageEvent> Search(UsageEventSearchCriteria criteria);
    }
}
