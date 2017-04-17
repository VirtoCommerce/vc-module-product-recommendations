using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations
{
    public class AzureRecommendationsClient : IAzureRecommendationsClient
    {
        private const string UserToItemRecommendationsUrlFormat = "{0}/models/{1}/recommend/user?userId={2}&buildId={3}&numberOfResults={4}&itemsIds={5}";
        private const string DefaultRequestApiKeyHeader = "Ocp-Apim-Subscription-Key";
        private const string ExceptionFormat = "Recommendations API {0} must be provided.";
        private readonly string _apiKey;

        public AzureRecommendationsClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string[]> GetCustomerRecommendationsAsync(string apiKey, string baseUrl, string modelId, string userId, string buildId, int numberOfResults, string[] productsIds)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new Exception(string.Format(ExceptionFormat, "URL"));
            if (string.IsNullOrEmpty(modelId))
                throw new Exception(string.Format(ExceptionFormat, "Model ID"));

            return await GetRecommendatinsAsync(apiKey, string.Format(UserToItemRecommendationsUrlFormat,
                baseUrl, modelId, userId, buildId, numberOfResults, productsIds != null ? string.Join(",", productsIds) : string.Empty));
        }

        private async Task<string[]> GetRecommendatinsAsync(string apiKey, string url)
        {
            var apiKeyWithFallback = string.IsNullOrEmpty(apiKey) ? _apiKey : apiKey;
            if (string.IsNullOrEmpty(apiKeyWithFallback))
                throw new Exception(string.Format(ExceptionFormat, "Key"));

            var result = new List<string>();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add(DefaultRequestApiKeyHeader, apiKeyWithFallback);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Error {0}: Failed to get recommendations, Reason: {1}", response.StatusCode, ExtractErrorInfo(response)));
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var recommendedItemSets = JsonConvert.DeserializeObject<RecommendedItemSets>(jsonString);

            if (recommendedItemSets != null && recommendedItemSets.Sets != null)
            {
                foreach (var recommendedItemSet in recommendedItemSets.Sets.Where(recommendedItemSet => recommendedItemSet.Items != null))
                {
                    result.AddRange(recommendedItemSet.Items.Select(recommendedItem => recommendedItem.Id));
                }
            }

            return result.ToArray();
        }

        private string ExtractErrorInfo(HttpResponseMessage response)
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