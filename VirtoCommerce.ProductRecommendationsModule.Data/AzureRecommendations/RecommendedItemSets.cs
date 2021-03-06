﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations
{
    public class RecommendedItemSets
    {
        [JsonProperty("recommendedItems")]
        public IEnumerable<RecommendedItemSet> Sets { get; set; }
    }
}
