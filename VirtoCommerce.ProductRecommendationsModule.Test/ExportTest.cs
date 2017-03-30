using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Moq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Export;
using Xunit;
using SearchCriteria = VirtoCommerce.Domain.Catalog.Model.SearchCriteria;
using SearchResult = VirtoCommerce.Domain.Catalog.Model.SearchResult;

namespace VirtoCommerce.ProductRecommendationsModule.Test
{
    public class ExportTest
    {
        private const int MaximumNumberOfUsageEvents = 5000000;

        [Fact]
        public void ExportCatalogTest()
        {
            var searchService = GetCatalogSearchService();
            var productService = GetProductService();
            var csvCatalogExporter = new CsvCatalogExporter(searchService, productService);
            ExportTestHelper(x => csvCatalogExporter.DoCatalogExport(x, "fileName", GetStore(), GetCatalog(), y => { }), GetWellFormedCatalogCsv());
        }

        [Fact]
        public void ExportUsageEventsTest()
        {
            var csvUsageEventsExporter = new CsvUsageEventsExporter(GetUsageEventService());
            ExportTestHelper(x => csvUsageEventsExporter.DoUsageEventsExport(x, "fileName", GetStore(), y => { }), GetWellFormedUsageEventsCsv());
        }

        private void ExportTestHelper(Action<Stream> exporter, string wellFormedCsv)
        {
            using (var stream = new MemoryStream())
            {
                exporter(stream);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var archive = new ZipArchive(stream))
                {
                    var entry = archive.Entries[0];
                    using (var entryStream = entry.Open())
                    {
                        using (var streamReader = new StreamReader(entryStream))
                        {
                            var csv = streamReader.ReadToEnd();
                            Assert.True(csv == wellFormedCsv);
                        }
                    }
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

        private IUsageEventService GetUsageEventService()
        {
            var usageEventService = new Mock<IUsageEventService>();
            usageEventService.Setup(x => x.Search(It.Is<UsageEventSearchCriteria>(y => y.Take == MaximumNumberOfUsageEvents)))
                .Returns<UsageEventSearchCriteria>(x => new GenericSearchResult<UsageEvent> { Results = GetUsageEvents() });
            return usageEventService.Object;
        }

        private Store GetStore()
        {
            var store = new Store
            {
                Settings = new[]
                {
                    new SettingEntry { Name = "Recommendations.UsageEvents.MaximumNumber", Value = MaximumNumberOfUsageEvents.ToString() }
                }
            };
            return store;
        }

        private Catalog GetCatalog()
        {
            var catalog = new Catalog();
            return catalog;
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

        private UsageEvent[] GetUsageEvents()
        {
            return new[]
            {
                new UsageEvent
                {
                    CustomerId = "Test user",
                    ItemId = "Test item",
                    EventType = UsageEventType.Click,
                    CreatedDate = DateTime.Parse("1970/01/01 00:00")
                },
                new UsageEvent
                {
                    CustomerId = "Test user",
                    ItemId = "Test item",
                    EventType = UsageEventType.AddShopCart,
                    CreatedDate = DateTime.Parse("1980/06/01 12:00")
                },
                new UsageEvent
                {
                    CustomerId = "Test user",
                    ItemId = "Test item",
                    EventType = UsageEventType.Purchase,
                    CreatedDate = DateTime.Parse("1990/01/01 00:00")
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

        private string GetWellFormedUsageEventsCsv()
        {
            var csvFormat = "{0},";
            var stringBuilder = new StringBuilder();
            var appendUsageEvent = new Action<UsageEvent>(usageEvent =>
            {
                stringBuilder.AppendFormat(csvFormat, usageEvent.CustomerId);
                stringBuilder.AppendFormat(csvFormat, usageEvent.ItemId);
                stringBuilder.AppendFormat(csvFormat, usageEvent.EventType.ToString());
                stringBuilder.AppendLine(usageEvent.CreatedDate.ToString("s"));

            });
            foreach (var usageEvent in GetUsageEvents())
            {
                appendUsageEvent(usageEvent);
            }
            return stringBuilder.ToString();
        }
    }
}
