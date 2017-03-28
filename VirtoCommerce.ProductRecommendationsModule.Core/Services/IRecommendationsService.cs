using System.Threading.Tasks;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Services
{
    public interface IRecommendationsService
    {
        Task<RecommendedItemSets> GetRecommendationsAsync(string storeId, string itemId, int numberOfResults);

        Task<RecommendedItemSets> GetCustomerRecommendationsAsync(string storeId, string customerId, int numberOfResults);
    }
}
