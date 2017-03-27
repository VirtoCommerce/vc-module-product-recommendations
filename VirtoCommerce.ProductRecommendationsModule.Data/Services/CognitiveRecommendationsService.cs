using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class CognitiveRecommendationsService : ICognitiveRecommendationsService
    {
        private readonly ISettingsManager _settingsManager;

        private const string BaseUriSettingName = "ProductRecommendations.BaseUri";
        private const string ApiKeySettingName = "ProductRecommendations.ApiKey";
        private const string ModelIdSettingName = "ProductRecommendations.ModelId";
        private const string BuildIdSettingName = "ProductRecommendations.BuildId";

        private const string ItemToItemRecommendationsUriFormat = "{0}/models/{1}/recommend/item?itemIds={2}&buildId={3}&numberOfResults={4}&minimalScore=0";
        private const string UserToItemRecommendationsUriFormat = "{0}/models/{1}/recommend/user?userId={2}&buildId={3}&numberOfResults={4}";

        private const string DefaultRequestApiKeyHeader = "Ocp-Apim-Subscription-Key";

        private readonly HttpClient _httpClient;

        public CognitiveRecommendationsService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
        }

        public async Task<RecommendedItemSetInfoList> GetRecommendationsAsync(string itemIds, int numberOfResults)
        {
            var settings = GetCognitiveRecommendationsServiceSettings(_settingsManager);

            var uri = string.Format(ItemToItemRecommendationsUriFormat, settings.BaseUri, settings.ModelId, itemIds, settings.BuildId, numberOfResults);

            return await GetRecommendatinsAsync(_httpClient, settings.ApiKey, uri);
        }

        public async Task<RecommendedItemSetInfoList> GetUserRecommendationsAsync(string userId, int numberOfResults)
        {
            var settings = GetCognitiveRecommendationsServiceSettings(_settingsManager);

            var uri = string.Format(UserToItemRecommendationsUriFormat, settings.BaseUri, settings.ModelId, userId, settings.BuildId, numberOfResults);

            return await GetRecommendatinsAsync(_httpClient, settings.ApiKey, uri);
        }

        private CongnitiveRecommendationsServiceSettings GetCognitiveRecommendationsServiceSettings(ISettingsManager settingsManager)
        {
            var baseUri = settingsManager.GetValue(BaseUriSettingName, string.Empty);
            var apiKey = settingsManager.GetValue(ApiKeySettingName, string.Empty);
            var modelId = settingsManager.GetValue(ModelIdSettingName, string.Empty);
            var buildId = settingsManager.GetValue(BuildIdSettingName, string.Empty);

            if (string.IsNullOrEmpty(baseUri))
                throw new Exception("Recommendations API BaseUri can't be null or empty.");

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Recommendations API Key can't be null or empty.");

            if (string.IsNullOrEmpty(modelId))
                throw new Exception("Recommendations API ModelId can't be null or empty.");

            if (string.IsNullOrEmpty(buildId))
                throw new Exception("Recommendations API BuildId can't be null or empty.");

            return new CongnitiveRecommendationsServiceSettings()
            {
                BaseUri = baseUri,
                ApiKey = apiKey,
                ModelId = modelId,
                BuildId = buildId
            };
        }

        private async Task<RecommendedItemSetInfoList> GetRecommendatinsAsync(HttpClient httpClient, string apiKey, string uri)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add(DefaultRequestApiKeyHeader, apiKey);

            var response = await httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error {response.StatusCode}: Failed to get recommendations, Reason: {ExtractErrorInfo(response)}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var recommendedItemSetInfoList = JsonConvert.DeserializeObject<RecommendedItemSetInfoList>(jsonString);

            return recommendedItemSetInfoList;
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
