using System.Threading.Tasks;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public interface ICognitiveRecommendationsService
    {
        Task<RecommendedItemSetInfoList> GetRecommendationsAsync(string itemIds, int numberOfResults);

        Task<RecommendedItemSetInfoList> GetUserRecommendationsAsync(string userId, int numberOfResults);
    }
}
