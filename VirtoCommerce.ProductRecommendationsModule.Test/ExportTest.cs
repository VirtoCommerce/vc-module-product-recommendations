using System;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.ProductRecommendationsModule.Web.Export;
using Xunit;

namespace VirtoCommerce.ProductRecommendationsModule.Test
{
    public class ExportTest
    {
        [Fact]
        public void ExportProductsTest()
        {
            var searchService = GetCatalogSearchService();
            var productService = GetProductService();
            var csvCatalogExporter = new CsvCatalogExporter(searchService, productService);
            using (var stream = new MemoryStream())
            {
                csvCatalogExporter.DoExport(stream, null, x => {});
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    Assert.True(streamReader.ReadToEnd() == GetWellFormedCsv());
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

        private string GetWellFormedCsv()
        {
            var csvFormat = "{0};";
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Id;Sku;CategoryCode");
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
    }
}
