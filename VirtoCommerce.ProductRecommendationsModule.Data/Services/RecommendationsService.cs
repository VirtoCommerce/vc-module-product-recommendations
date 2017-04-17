using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.AzureRecommendations;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IStoreService _storeService;
        private readonly IAzureRecommendationsClient _azureRecommendationsClient;

        public RecommendationsService(IStoreService storeService, IAzureRecommendationsClient azureRecommendationsClient)
        {
            _storeService = storeService;
            _azureRecommendationsClient = azureRecommendationsClient;
        }

        public async Task<string[]> GetRecommendationsAsync(RecommendationEvalContext context)
        {
            AzureRecommendationType azureRecommendationType;
            var isAzureRecommendations = Enum.TryParse(context.Type, out azureRecommendationType);
            if (!isAzureRecommendations)
            {
                throw new NotSupportedException();
            }

            var settins = GetRecommendationsSettings(context.StoreId);

            string[] result;
            if (azureRecommendationType == AzureRecommendationType.User2Item)
            {
                result = await _azureRecommendationsClient.GetCustomerRecommendationsAsync(settins.ApiKey, settins.BaseUrl, context.ModelId,
                        context.UserId, context.BuildId, context.Take, context.ProductIds.ToArray());
            }
            else
            {
                throw new NotSupportedException();
            }
            return result;
        }

        private RecommendationServiceSettings GetRecommendationsSettings(string storeId)
        {
            var store = _storeService.GetById(storeId);

            var apiKey = store.Settings.GetSettingValue<string>("Recommendations.ApiKey", null);
            var baseUrl = store.Settings.GetSettingValue<string>("Recommendations.BaseUrl", null);

            return new RecommendationServiceSettings(apiKey, baseUrl);
        }
    }
}
