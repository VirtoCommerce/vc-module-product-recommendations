using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    internal class RecommendationServiceSettings
    {
        public RecommendationServiceSettings(string apiKey, string baseUrl, string modelId, string buildId)
        {
            ApiKey = apiKey;
            BaseUrl = baseUrl;
            ModelId = modelId;
            BuildId = buildId;
        }

        public string ApiKey { get; }

        public string BaseUrl { get; }

        public string ModelId { get; }

        public string BuildId { get; }
    }
}
