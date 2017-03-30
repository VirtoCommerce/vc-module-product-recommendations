using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;
using SearchCriteria = VirtoCommerce.Domain.Catalog.Model.SearchCriteria;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvCatalogExporter : CsvExporter
    {
        private readonly IStoreService _storeService;
        private readonly ICatalogService _catalogService;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IItemService _productService;

        public CsvCatalogExporter(IStoreService storeService, ICatalogService catalogService, ICatalogSearchService catalogSearchService, IItemService productService,
            IPushNotificationManager pushNotifier, IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver) : base(pushNotifier, blobStorageProvider, blobUrlResolver)
        {
            _storeService = storeService;
            _catalogService = catalogService;
            _catalogSearchService = catalogSearchService;
            _productService = productService;
        }

        public void DoCatalogExport(string storeId, ExportPushNotification notification)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new NullReferenceException("store");
            }
            var catalog = _catalogService.GetById(store.Catalog);
            if (catalog == null)
            {
                throw new NullReferenceException("catalog");
            }

            DoExport(catalog.Name, store.Settings.GetSettingValue("Recommendations.Catalog.ChunkSize", 200) * 1024 * 1024,
                "products", () => LoadProducts(store, catalog).Select(x => new CsvProduct(x)).ToArray(), new CsvProductMap(), notification);
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