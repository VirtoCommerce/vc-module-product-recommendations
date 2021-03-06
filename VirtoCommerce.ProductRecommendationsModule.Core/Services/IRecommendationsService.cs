﻿using System.Threading.Tasks;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Core.Services
{
    public interface IRecommendationsService
    {
        Task<string[]> GetRecommendationsAsync(RecommendationEvalContext context);
    }
}
