using System;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.CognitiveServices;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IStoreService _storeService;

        public RecommendationsService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<string[]> GetCustomerRecommendationsAsync(string storeId, string customerId, int numberOfResults)
        {
            return await GetRecommendationsAsync(RecommendationsApi.GetCustomerRecommendationsAsync, storeId, customerId, numberOfResults);
        }

        private async Task<string[]> GetRecommendationsAsync(Func<string, object[], Task<string[]>> apiCall, string storeId, string entityId, int numberOfResults)
        {
            var store = _storeService.GetById(storeId);

            var apiKey = store.Settings.GetSettingValue<string>("Recommendations.ApiKey", null);
            var baseUri = store.Settings.GetSettingValue<string>("Recommendations.BaseUrl", null);
            var modelId = store.Settings.GetSettingValue<string>("Recommendations.ModelId", null);
            var buildId = store.Settings.GetSettingValue<string>("Recommendations.BuildId", null);

            var exceptionFormat = "Recommendations API {0} must be provided.";

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception(string.Format(exceptionFormat, "Key"));
            if (string.IsNullOrEmpty(baseUri))
                throw new Exception(string.Format(exceptionFormat, "URL"));
            if (string.IsNullOrEmpty(modelId))
                throw new Exception(string.Format(exceptionFormat, "Model ID"));
            if (string.IsNullOrEmpty(buildId))
                throw new Exception(string.Format(exceptionFormat, "Build ID"));

            return await apiCall(apiKey, new object[] { baseUri, modelId, entityId, buildId, numberOfResults });
        }
    }
}
