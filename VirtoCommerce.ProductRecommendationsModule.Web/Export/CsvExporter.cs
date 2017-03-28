using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ProductRecommendationsModule.Core.Model;
using VirtoCommerce.ProductRecommendationsModule.Core.Model.Search;
using VirtoCommerce.ProductRecommendationsModule.Core.Services;
using SearchCriteria = VirtoCommerce.Domain.Catalog.Model.SearchCriteria;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public sealed class CsvExporter
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IItemService _productService;
        private readonly IUsageEventService _usageEventService;

        public CsvExporter(ICatalogSearchService catalogSearchService, IItemService productService, IUsageEventService usageEventService)
        {
            _catalogSearchService = catalogSearchService;
            _productService = productService;
            _usageEventService = usageEventService;
        }

        public void DoCatalogExport(Stream outStream, Store store, Catalog catalog, Action<ExportImportProgressInfo> progressCallback)
        {
            DoExport(outStream, "products", () => LoadProducts(store, catalog).Select(x => new CsvProduct(x)).ToArray(), new CsvProductMap(), progressCallback);
        }

        public void DoUsageEventsExport(Stream outStream, Store store, Action<ExportImportProgressInfo> progressCallback)
        {
            DoExport(outStream, "events", () => LoadEvents(store).Select(x => new CsvUsageEvent(x)).ToArray(), new CsvUsageEventMap(), progressCallback);
        }
        
        [CLSCompliant(false)]
        public void DoExport<TCsvClass, TClass>(Stream outStream, string entitiesType, Func<ICollection<TCsvClass>> entityFactory, CsvClassMap<TClass> entityClassMap, Action<ExportImportProgressInfo> progressCallback)
        {
            var prodgressInfo = new ExportImportProgressInfo
            {
                Description = string.Format("loading {0}...", entitiesType)
            };

            var streamWriter = new StreamWriter(outStream, new UTF8Encoding(false), 1024, true) { AutoFlush = true };
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                //Notification
                progressCallback(prodgressInfo);

                //Load all entities to export
                var entities = entityFactory();

                csvWriter.Configuration.Delimiter = ",";
                csvWriter.Configuration.RegisterClassMap(entityClassMap);

                prodgressInfo.TotalCount = entities.Count;
                var notifyProductSizeLimit = 50;
                var counter = 0;
                foreach (var entity in entities)
                {
                    try
                    {
                        csvWriter.WriteRecord(entity);
                    }
                    catch (Exception ex)
                    {
                        prodgressInfo.Errors.Add(ex.ToString());
                        progressCallback(prodgressInfo);
                    }

                    //Raise notification each notifyProductSizeLimit products
                    counter++;
                    prodgressInfo.ProcessedCount = counter;
                    prodgressInfo.Description = string.Format("{0} of {1} {2} processed", prodgressInfo.ProcessedCount, prodgressInfo.TotalCount, entitiesType);
                    if (counter % notifyProductSizeLimit == 0 || counter == prodgressInfo.TotalCount)
                    {
                        progressCallback(prodgressInfo);
                    }
                }
            }
        }

        private ICollection<CatalogProduct> LoadProducts(Store store, Catalog catalog)
        {
            // TODO: Implement product count restriction from Catalog.MaximumNumber setting
            var retVal = new List<CatalogProduct>();

            var productsIds = _catalogSearchService.Search(new SearchCriteria
            {
                CatalogId = catalog.Id, SearchInChildren = true, Skip = 0, Take = int.MaxValue, ResponseGroup = SearchResponseGroup.WithProducts
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

        private ICollection<UsageEvent> LoadEvents(Store store)
        {
            var maximumNumberOfEvents = int.Parse(store.Settings.First(x => x.Name == "Recommendations.UsageEvents.MaximumNumber").Value);
            return _usageEventService.Search(new UsageEventSearchCriteria
            {
                StoreId = store.Id,
                Sort = "created:desc",
                Skip = 0,
                Take = maximumNumberOfEvents
            }).Results;
        }
    }
}