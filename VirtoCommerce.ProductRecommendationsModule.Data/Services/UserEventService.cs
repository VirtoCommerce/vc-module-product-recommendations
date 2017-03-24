using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class UserEventService: IUserEventService
    {
        public void AddEvent(UserEvent userEvent)
        {
            //todo
        }

        public UserEvent[] GetUserEvents(SearchUserEventCriteria criteria)
        {
            //todo

            return new UserEvent[]
            {
                new UserEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    ItemId = Guid.NewGuid().ToString(),
                    EventType = "Click",
                    Created = DateTime.UtcNow
                }
            };
        }
    }
}
