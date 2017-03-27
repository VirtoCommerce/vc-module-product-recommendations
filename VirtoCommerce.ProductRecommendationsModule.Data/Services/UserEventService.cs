using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;
using VirtoCommerce.ProductRecommendationsModule.Data.Repositories;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class UserEventService: IUserEventService
    {
        private readonly Func<IUserEventRepository> _userEventRepositoryFactory;

        public UserEventService(Func<IUserEventRepository> userEventRepositoryFactory)
        {
            _userEventRepositoryFactory = userEventRepositoryFactory;
        }

        public void Add(UserEvent userEvent)
        {
            using (var repository = _userEventRepositoryFactory())
            {
                repository.Add(userEvent);
                repository.UnitOfWork.Commit();
            }
        }

        public UserEvent[] Search(SearchUserEventCriteria criteria)
        {
            using (var repository = _userEventRepositoryFactory())
            {
                var query = repository.UserEvents;
                if (!string.IsNullOrEmpty(criteria.StoreId))
                {
                    query = query.Where(x => criteria.StoreId == x.StoreId);
                }
                return query.Reverse().Skip(criteria.Skip).Take(criteria.Take).ToArray();
            }
        }
    }
}
