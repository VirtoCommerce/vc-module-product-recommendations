using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public sealed class CsvCatalogExporter
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IItemService _productService;

        public CsvCatalogExporter(ICatalogSearchService catalogSearchService, IItemService productService)
        {
            _catalogSearchService = catalogSearchService;
            _productService = productService;
        }

        public void DoExport(Stream outStream, string catalogId, Action<ExportImportProgressInfo> progressCallback)
        {
            var prodgressInfo = new ExportImportProgressInfo
            {
                Description = "loading products..."
            };

            var streamWriter = new StreamWriter(outStream, Encoding.UTF8, 1024, true) { AutoFlush = true };
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                //Notification
                progressCallback(prodgressInfo);

                //Load all products to export
                var products = LoadProducts(catalogId);

                csvWriter.Configuration.Delimiter = ";";

                prodgressInfo.TotalCount = products.Count;
                var notifyProductSizeLimit = 50;
                var counter = 0;
                foreach (var product in products)
                {
                    try
                    {
                        var csvProduct = new CsvProduct(product);
                        csvWriter.WriteRecord(csvProduct);
                    }
                    catch (Exception ex)
                    {
                        prodgressInfo.Errors.Add(ex.ToString());
                        progressCallback(prodgressInfo);
                    }

                    //Raise notification each notifyProductSizeLimit products
                    counter++;
                    prodgressInfo.ProcessedCount = counter;
                    prodgressInfo.Description = string.Format("{0} of {1} products processed", prodgressInfo.ProcessedCount, prodgressInfo.TotalCount);
                    if (counter % notifyProductSizeLimit == 0 || counter == prodgressInfo.TotalCount)
                    {
                        progressCallback(prodgressInfo);
                    }
                }
            }
        }

        private List<CatalogProduct> LoadProducts(string catalogId)
        {
            var retVal = new List<CatalogProduct>();

            var productsIds = _catalogSearchService.Search(new SearchCriteria
            {
                CatalogId = catalogId, SearchInChildren = true, Skip = 0, Take = int.MaxValue, ResponseGroup = SearchResponseGroup.WithProducts
            }).Products.Select(x => x.Id).ToArray();
            var products = _productService.GetByIds(productsIds.Distinct().ToArray(), ItemResponseGroup.ItemInfo | ItemResponseGroup.Variations);

            foreach (var product in products)
            {
                retVal.Add(product);
                if (product.Variations != null)
                {
                    retVal.AddRange(product.Variations);
                }
            }

            return retVal;
        }
    }
}