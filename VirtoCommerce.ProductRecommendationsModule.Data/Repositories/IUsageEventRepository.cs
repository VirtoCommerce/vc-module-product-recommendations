using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Repositories
{
    public interface IUsageEventRepository : IRepository
    {
        IQueryable<UsageEventEntity> UsageEvents { get; }
    }
}
