using System.Linq;
using Moq;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ProductRecommendationsModule.Test
{
    public class RecommendationsServiceTest
    {
        [Fact]
        public void UserToItemRecommendationsTest()
        {
            var service = GetRecommendationsService();
            var result = service.GetCustomerRecommendationsAsync(null, "000340019B9C4024", 5).Result;
            Assert.True(result.Count() == 5);
        }

        private IRecommendationsService GetRecommendationsService()
        {
            return new RecommendationsService(GetStoreService());
        }

        private IStoreService GetStoreService()
        {
            var storeService = new Mock<IStoreService>();
            storeService.Setup(x => x.GetById(It.IsAny<string>()))
                .Returns<string>(x => new Store
                {
                    Settings = new[]
                    {
                        new SettingEntry { Name = "Recommendations.BaseUrl", Value = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0" },
                        new SettingEntry { Name = "Recommendations.ApiKey", Value = "" },
                        new SettingEntry { Name = "Recommendations.ModelId", Value = "1313ec3b-7043-4357-b4ab-5aeabe7df0b4" },
                        new SettingEntry { Name = "Recommendations.BuildId", Value = "1618199" },
                    }
                });
            return storeService.Object;
        }
    }
}
