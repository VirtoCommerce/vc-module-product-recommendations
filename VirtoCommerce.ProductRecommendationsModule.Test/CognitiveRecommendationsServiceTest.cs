using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ProductRecommendationsModule.Test
{
    public class CognitiveRecommendationsServiceTest
    {
        [Fact]
        public void ItemToItemRecommendationsTest()
        {
            var service = GetCognitiveRecommendationsService();
            var result = service.GetRecommendationsAsync("AAA-01148", 5).Result;
            Assert.True(result.RecommendedItemSetInfo.Count() == 5);
        }

        [Fact]
        public void UserToItemRecommendationsTest()
        {
            var service = GetCognitiveRecommendationsService();
            var result = service.GetUserRecommendationsAsync("000340019B9C4024", 5).Result;
            Assert.True(result.RecommendedItemSetInfo.Count() == 5);
        }

        private ICognitiveRecommendationsService GetCognitiveRecommendationsService()
        {
            var settingManager = new Mock<ISettingsManager>();

            settingManager.Setup(s => s.GetValue("ProductRecommendations.BaseUri", It.IsAny<string>())).Returns("https://westus.api.cognitive.microsoft.com/recommendations/v4.0" );
            settingManager.Setup(s => s.GetValue("ProductRecommendations.ApiKey", It.IsAny<string>())).Returns("");
            settingManager.Setup(s => s.GetValue("ProductRecommendations.ModelId", It.IsAny<string>())).Returns("1313ec3b-7043-4357-b4ab-5aeabe7df0b4");
            settingManager.Setup(s => s.GetValue("ProductRecommendations.BuildId", It.IsAny<string>())).Returns("1618199");

            return new CognitiveRecommendationsService(settingManager.Object);
        }
    }
}
