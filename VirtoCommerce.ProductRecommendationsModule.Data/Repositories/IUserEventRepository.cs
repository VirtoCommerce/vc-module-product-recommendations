using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Repositories
{
    public interface IUserEventRepository : IRepository
    {
        IQueryable<UserEvent> UserEvents { get; }

        UserEvent[] GetUserEventsByIds(string[] userEventsIds);
    }
}
