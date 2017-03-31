using System;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

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
            var settins = GetRecommendationsSettings(storeId);
            return await _azureRecommendationsClient.GetCustomerRecommendationsAsync(settins.ApiKey, settins.BaseUrl, settins.ModelId, customerId, settins.BuildId, numberOfResults, productsIds);
        }

        private RecommendationServiceSettings GetRecommendationsSettings(string storeId)
        {
            var store = _storeService.GetById(storeId);

            var apiKey = store.Settings.GetSettingValue<string>("Recommendations.ApiKey", null);
            var baseUrl = store.Settings.GetSettingValue<string>("Recommendations.BaseUrl", null);
            var modelId = store.Settings.GetSettingValue<string>("Recommendations.ModelId", null);
            var buildId = store.Settings.GetSettingValue<string>("Recommendations.BuildId", null);

            var exceptionFormat = "Recommendations API {0} must be provided.";

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception(string.Format(exceptionFormat, "Key"));
            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception(string.Format(exceptionFormat, "URL"));
            if (string.IsNullOrEmpty(modelId))
                throw new Exception(string.Format(exceptionFormat, "Model ID"));
            if (string.IsNullOrEmpty(buildId))
                throw new Exception(string.Format(exceptionFormat, "Build ID"));

            return new RecommendationServiceSettings(apiKey, baseUrl, modelId, buildId);
        }
    }
}
