using System.Threading.Tasks;

namespace VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations
{
    public interface IAzureRecommendationsClient
    {
        Task<string[]> GetCustomerRecommendationsAsync(string apiKey, string baseUrl, string modelId, string userId, string buildId, int numberOfResults);
    }
}