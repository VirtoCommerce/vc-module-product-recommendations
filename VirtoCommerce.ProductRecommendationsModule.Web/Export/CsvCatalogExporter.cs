using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using SearchCriteria = VirtoCommerce.Domain.Catalog.Model.SearchCriteria;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvCatalogExporter : CsvExporter
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IItemService _productService;

        public CsvCatalogExporter(ICatalogSearchService catalogSearchService, IItemService productService)
        {
            _catalogSearchService = catalogSearchService;
            _productService = productService;
        }
        public void DoCatalogExport(Stream outStream, string fileName, Store store, Catalog catalog, Action<ExportImportProgressInfo> progressCallback)
        {
            DoExport(outStream, fileName, store.Settings.GetSettingValue("Recommendations.Catalog.ChunkSize", 200) * 1024 * 1024,
                "products", () => LoadProducts(store, catalog).Select(x => new CsvProduct(x)).ToArray(), new CsvProductMap(), progressCallback);
        }
        
        private ICollection<CatalogProduct> LoadProducts(Store store, Catalog catalog)
        {
            // TODO: Implement product count restriction from Catalog.MaximumNumber setting
            var retVal = new List<CatalogProduct>();

            var productsIds = _catalogSearchService.Search(new SearchCriteria
            {
                CatalogId = catalog.Id,
                SearchInChildren = true,
                Skip = 0,
                Take = int.MaxValue,
                ResponseGroup = SearchResponseGroup.WithProducts
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