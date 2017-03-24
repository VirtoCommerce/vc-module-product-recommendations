using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AddEvent(UserEvent userEvent)
        {
            using (var repository = _userEventRepositoryFactory())
            {
                repository.Add(userEvent);
                repository.UnitOfWork.Commit();
            }
        }

        public UserEvent[] GetUserEvents(SearchUserEventCriteria criteria)
        {
            using (var repository = _userEventRepositoryFactory())
            {
                return repository.GetUserEvents(criteria.CreatedFrom, criteria.CreatedTo);
            }
        }
    }
}
