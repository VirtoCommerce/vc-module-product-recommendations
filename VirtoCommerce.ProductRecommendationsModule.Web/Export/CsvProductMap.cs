using System;
using CsvHelper.Configuration;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    [CLSCompliant(false)]
    public class CsvProductMap: CsvClassMap<CsvProduct>
    {
        public CsvProductMap()
        {
            Map(x => x.Id).Index(0);
            Map(x => x.Sku).Index(1);
            Map(x => x.CategoryCode).Index(2);
        }
    }
}