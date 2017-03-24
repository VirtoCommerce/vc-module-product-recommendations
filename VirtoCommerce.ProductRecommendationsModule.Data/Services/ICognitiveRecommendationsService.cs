using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public interface ICognitiveRecommendationsService
    {
        RecommendedItemSetInfoList GetRecommendations(string modelId, long? buildId, string itemIds, int take);

        RecommendedItemSetInfoList GetUserRecommendations(string modelId, long? buildId, string userId, int take);
    }
}
