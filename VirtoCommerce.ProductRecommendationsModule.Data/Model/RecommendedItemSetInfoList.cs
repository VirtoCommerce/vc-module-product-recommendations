using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class RecommendedItemInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }
    }

    public class RecommendedItemSetInfo
    {
        public RecommendedItemSetInfo()
        {
            Items = new List<RecommendedItemInfo>();
        }

        [JsonProperty("items")]
        public IEnumerable<RecommendedItemInfo> Items { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("reasoning")]
        public IEnumerable<string> Reasoning { get; set; }
    }


    public class RecommendedItemSetInfoList
    {
        [JsonProperty("recommendedItems")]
        public IEnumerable<RecommendedItemSetInfo> RecommendedItemSetInfo { get; set; }
    }
}
