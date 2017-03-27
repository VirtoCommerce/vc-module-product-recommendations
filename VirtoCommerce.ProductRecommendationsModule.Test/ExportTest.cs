using System;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;
using VirtoCommerce.ProductRecommendationsModule.Data.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Export;
using Xunit;

namespace VirtoCommerce.ProductRecommendationsModule.Test
{
    public class ExportTest
    {
        private const int MaximumNumberOfUserEvents = 5000000;

        [Fact]
        public void ExportCatalogTest()
        {
            var searchService = GetCatalogSearchService();
            var productService = GetProductService();
            var csvCatalogExporter = new CsvExporter(searchService, productService, null, null);
            using (var stream = new MemoryStream())
            {
                csvCatalogExporter.DoCatalogExport(stream, null, x => {});
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    var csv = streamReader.ReadToEnd();
                    var wellFormedCsv = GetWellFormedCatalogCsv();
                    Assert.True(csv == wellFormedCsv);
                }
            }
        }

        [Fact]
        public void ExportUserEventsTest()
        {
            var csvCatalogExporter = new CsvExporter(null, null, GetUserEventService(), GetSettingManager());
            using (var stream = new MemoryStream())
            {
                csvCatalogExporter.DoUserEventsExport(stream, null, x => { });
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    var csv = streamReader.ReadToEnd();
                    var wellFormedCsv = GetWellFormedUserEventsCsv();
                    Assert.True(csv == wellFormedCsv);
                }
            }
        }

        private ICatalogSearchService GetCatalogSearchService()
        {
            var catalogSearchService = new Mock<ICatalogSearchService>();
            catalogSearchService.Setup(x => x.Search(It.IsAny<SearchCriteria>()))
                .Returns<SearchCriteria>(x => new SearchResult { Products = GetProducts() });
            return catalogSearchService.Object;
        }

        private IItemService GetProductService()
        {
            var productService = new Mock<IItemService>();
            productService.Setup(x => x.GetByIds(It.Is<string[]>(y => y.SequenceEqual(new[] { "1", "2", "3" })), It.IsAny<ItemResponseGroup>(), It.IsAny<string>()))
                .Returns<string[], ItemResponseGroup, string>((x, y, z) => GetProductsWithVariations());
            return productService.Object;
        }

        private IUserEventService GetUserEventService()
        {
            var userEventService = new Mock<IUserEventService>();
            userEventService.Setup(x => x.Search(It.Is<SearchUserEventCriteria>(y => y.Take == MaximumNumberOfUserEvents)))
                .Returns<SearchUserEventCriteria>(x => GetUserEvents());
            return userEventService.Object;
        }

        private ISettingsManager GetSettingManager()
        {
            var settingManager = new Mock<ISettingsManager>();
            settingManager.Setup(x => x.GetValue(It.Is<string>(y => string.Compare(y, "ProductRecommendations.UserEvents.MaximumNumber", StringComparison.OrdinalIgnoreCase) == 0), It.IsAny<int>()))
                .Returns<string, int>((x, y) => MaximumNumberOfUserEvents);
            return settingManager.Object;
        }

        private Category[] GetCategories()
        {
            return new[]
            {
                new Category { Code = "First" },
                new Category { Code = "Second" }
            };
        }

        private CatalogProduct[] GetProducts()
        {
            var categories = GetCategories();
            return new[]
            {
                new CatalogProduct { Id = "1", Code = "#1", Category = categories[0] },
                new CatalogProduct { Id = "2", Code = "#2", Category = categories[0] },
                new CatalogProduct { Id = "3", Code = "#3", Category = categories[1] }
            };
        }

        private CatalogProduct[] GetProductsWithVariations()
        {
            var products = GetProducts();
            var category = GetCategories()[0];
            products[1].Variations = new[]
            {
                new CatalogProduct { Id = "2.1", Code = "#2.1", Category = category },
                new CatalogProduct { Id = "2.2", Code = "#2.2", Category = category },
                new CatalogProduct { Id = "2.3", Code = "#2.3", Category = category }
            };
            return products;
        }

        private UserEvent[] GetUserEvents()
        {
            return new[]
            {
                new UserEvent
                {
                    UserId = "Test user",
                    ItemId = "Test item",
                    EventType = UserEventType.Click,
                    Created = DateTime.Parse("1970/01/01 00:00")
                },
                new UserEvent
                {
                    UserId = "Test user",
                    ItemId = "Test item",
                    EventType = UserEventType.AddShopCart,
                    Created = DateTime.Parse("1980/06/01 12:00")
                },
                new UserEvent
                {
                    UserId = "Test user",
                    ItemId = "Test item",
                    EventType = UserEventType.Purchase,
                    Created = DateTime.Parse("1990/01/01 00:00")
                }
            };
        }
        
        private string GetWellFormedCatalogCsv()
        {
            var csvFormat = "{0},";
            var stringBuilder = new StringBuilder();
            var appendProduct = new Action<CatalogProduct>(product =>
            {
                stringBuilder.AppendFormat(csvFormat, product.Id);
                stringBuilder.AppendFormat(csvFormat, product.Code);
                stringBuilder.AppendLine(product.Category.Code);

            });
            foreach (var product in GetProductsWithVariations())
            {
                appendProduct(product);
                foreach (var variation in product.Variations ?? Enumerable.Empty<CatalogProduct>())
                {
                    appendProduct(variation);
                }
            }
            return stringBuilder.ToString();
        }

        private string GetWellFormedUserEventsCsv()
        {
            var csvFormat = "{0},";
            var stringBuilder = new StringBuilder();
            var appendUserEvent = new Action<UserEvent>(userEvent =>
            {
                stringBuilder.AppendFormat(csvFormat, userEvent.UserId);
                stringBuilder.AppendFormat(csvFormat, userEvent.ItemId);
                stringBuilder.AppendFormat(csvFormat, userEvent.EventType.ToString());
                stringBuilder.AppendLine(userEvent.Created.ToString("s"));

            });
            foreach (var userEvent in GetUserEvents())
            {
                appendUserEvent(userEvent);
            }
            return stringBuilder.ToString();
        }
    }
}
