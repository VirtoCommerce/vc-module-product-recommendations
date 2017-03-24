using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class CognitiveRecommendationsService: ICognitiveRecommendationsService
    {
        public RecommendedItemSetInfoList GetRecommendations(string modelId, long? buildId, string itemIds, int take)
        {
            throw new NotImplementedException();
        }

        public RecommendedItemSetInfoList GetUserRecommendations(string modelId, long? buildId, string userId, int take)
        {
            throw new NotImplementedException();
        }
    }
}
