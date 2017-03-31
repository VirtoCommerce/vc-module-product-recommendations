using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations
{
    public class RecommendedItemSet
    {
        public RecommendedItemSet()
        {
            Items = new List<RecommendedItem>();
        }

        [JsonProperty("items")]
        public IEnumerable<RecommendedItem> Items { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("reasoning")]
        public IEnumerable<string> Reasoning { get; set; }
    }
}