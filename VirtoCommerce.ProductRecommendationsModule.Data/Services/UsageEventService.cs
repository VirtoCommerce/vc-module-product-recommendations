using System;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;
using VirtoCommerce.ProductRecommendationsModule.Data.Repositories;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class UsageEventService: ServiceBase, IUsageEventService
    {
        private readonly Func<IUsageEventRepository> _usageEventRepositoryFactory;

        public UsageEventService(Func<IUsageEventRepository> usageEventRepositoryFactory)
        {
            _usageEventRepositoryFactory = usageEventRepositoryFactory;
        }

        public void Add(UsageEvent[] usageEvents)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _usageEventRepositoryFactory())
            {
                foreach (var usageEvent in usageEvents)
                {
                    repository.Add(AbstractTypeFactory<UsageEventEntity>.TryCreateInstance().FromModel(usageEvent, pkMap));
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public GenericSearchResult<UsageEvent> Search(UsageEventSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<UsageEvent>();
            using (var repository = _usageEventRepositoryFactory())
            {
                var query = repository.UsageEvents;
                if (!string.IsNullOrEmpty(criteria.StoreId))
                {
                    query = query.Where(x => criteria.StoreId == x.StoreId);
                }
                query = query.OrderBySortInfos(criteria.SortInfos);

                retVal.TotalCount = query.Count();
                retVal.Results = query.Skip(criteria.Skip).Take(criteria.Take).ToArray().Select(x => x.ToModel(AbstractTypeFactory<UsageEvent>.TryCreateInstance())).ToArray();

                return retVal;
            }
        }
    }
}
