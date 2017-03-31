using System;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IStoreService _storeService;
        private readonly IAzureRecommendationsClient _azureRecommendationsClient;

        public RecommendationsService(IStoreService storeService, IAzureRecommendationsClient azureRecommendationsClient)
        {
            _storeService = storeService;
            _azureRecommendationsClient = azureRecommendationsClient;
        }

        public async Task<string[]> GetCustomerRecommendationsAsync(string storeId, string customerId, string[] productsIds, int numberOfResults)
        {
            string apiKey, baseUrl, modelId, buildId;
            GetRecommendationsSettings(storeId, out apiKey, out baseUrl, out modelId, out buildId);
            return await _azureRecommendationsClient.GetCustomerRecommendationsAsync(apiKey, baseUrl, modelId, customerId, buildId, numberOfResults, productsIds);
        }

        private void GetRecommendationsSettings(string storeId, out string apiKey, out string baseUrl, out string modelId, out string buildId)
        {
            var store = _storeService.GetById(storeId);

            apiKey = store.Settings.GetSettingValue<string>("Recommendations.ApiKey", null);
            baseUrl = store.Settings.GetSettingValue<string>("Recommendations.BaseUrl", null);
            modelId = store.Settings.GetSettingValue<string>("Recommendations.ModelId", null);
            buildId = store.Settings.GetSettingValue<string>("Recommendations.BuildId", null);

            var exceptionFormat = "Recommendations API {0} must be provided.";

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception(string.Format(exceptionFormat, "Key"));
            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception(string.Format(exceptionFormat, "URL"));
            if (string.IsNullOrEmpty(modelId))
                throw new Exception(string.Format(exceptionFormat, "Model ID"));
            if (string.IsNullOrEmpty(buildId))
                throw new Exception(string.Format(exceptionFormat, "Build ID"));
        }
    }
}
