using System;
using CsvHelper.Configuration;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    // Why we need this?
    // Look at CvsExporter class to answer
    [CLSCompliant(false)]
    public class CsvProductMap: CsvClassMap<CsvProduct>
    {
        public CsvProductMap()
        {
            // https://westus.dev.cognitive.microsoft.com/docs/services/Recommendations.V4.0/operations/56f316efeda5650db055a3e1
            // Item Id
            Map(x => x.Id).Index(0);
            // Item Name
            Map(x => x.Sku).Index(1);
            // Item Category
            Map(x => x.CategoryCode).Index(2);
        }
    }
}