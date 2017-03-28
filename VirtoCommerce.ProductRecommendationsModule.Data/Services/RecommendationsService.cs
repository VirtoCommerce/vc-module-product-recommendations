using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IStoreService _storeService;

        private const string ItemToItemRecommendationsUrlFormat = "{0}/models/{1}/recommend/item?itemIds={2}&buildId={3}&numberOfResults={4}&minimalScore=0";
        private const string UserToItemRecommendationsUrlFormat = "{0}/models/{1}/recommend/user?userId={2}&buildId={3}&numberOfResults={4}";

        private const string DefaultRequestApiKeyHeader = "Ocp-Apim-Subscription-Key";

        private readonly HttpClient _httpClient;

        public RecommendationsService(IStoreService storeService)
        {
            _storeService = storeService;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
        }

        public async Task<RecommendedItemSets> GetRecommendationsAsync(string storeId, string[] itemIds, int numberOfResults)
        {
            return await GetRecommendationsAsync(ItemToItemRecommendationsUrlFormat, storeId, string.Join(",", itemIds), numberOfResults);
        }

        public async Task<RecommendedItemSets> GetCustomerRecommendationsAsync(string storeId, string customerId, int numberOfResults)
        {
            return await GetRecommendationsAsync(UserToItemRecommendationsUrlFormat, storeId, customerId, numberOfResults);
        }

        private async Task<RecommendedItemSets> GetRecommendationsAsync(string urlFormat, string storeId, string entityId, int numberOfResults)
        {
            var store = _storeService.GetById(storeId);

            var apiKeySetting = store.Settings.First(x => x.Name == "Recommendations.ApiKey");
            var baseUrlSetting = store.Settings.First(x => x.Name == "Recommendations.BaseUrl");
            var modelIdSetting = store.Settings.First(x => x.Name == "Recommendations.ModelId");
            var buildIdSetting = store.Settings.First(x => x.Name == "Recommendations.BuildId");

            var baseUri = baseUrlSetting.Value;
            var apiKey = apiKeySetting.Value;
            var modelId = modelIdSetting.Value;
            var buildId = buildIdSetting.Value;

            var exceptionFormat = "{0} must be provided.";
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception(string.Format(exceptionFormat, apiKeySetting.Title));
            if (string.IsNullOrEmpty(baseUri))
                throw new Exception(string.Format(exceptionFormat, baseUrlSetting.Title));
            if (string.IsNullOrEmpty(modelId))
                throw new Exception(string.Format(exceptionFormat, modelIdSetting.Title));
            if (string.IsNullOrEmpty(buildId))
                throw new Exception(string.Format(exceptionFormat, buildIdSetting.Title));

            var url = string.Format(urlFormat, baseUri, modelId, entityId, buildId, numberOfResults);

            return await GetRecommendatinsAsync(_httpClient, apiKey, url);
        }

        private async Task<RecommendedItemSets> GetRecommendatinsAsync(HttpClient httpClient, string apiKey, string url)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add(DefaultRequestApiKeyHeader, apiKey);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Error {0}: Failed to get recommendations, Reason: {1}", response.StatusCode, ExtractErrorInfo(response)));
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RecommendedItemSets>(jsonString);
        }

        private static string ExtractErrorInfo(HttpResponseMessage response)
        {
            string detailedReason = null;

            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }

            var errorMsg = detailedReason == null ? response.ReasonPhrase : response.ReasonPhrase + "->" + detailedReason;

            return errorMsg;
        }
    }
}
