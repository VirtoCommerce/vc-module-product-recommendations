using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    internal class RecommendationServiceSettings
    {
        public RecommendationServiceSettings(string apiKey, string baseUrl)
        {
            ApiKey = apiKey;
            BaseUrl = baseUrl;
        }

        public string ApiKey { get; }

        public string BaseUrl { get; }
    }
}
