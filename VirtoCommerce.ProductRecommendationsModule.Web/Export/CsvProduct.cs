using VirtoCommerce.Domain.Catalog.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvProduct
    {
        public CsvProduct(CatalogProduct catalogProduct)
        {
            Id = catalogProduct.Id;
            Sku = catalogProduct.Code;
            CategoryCode = catalogProduct.Category.Code;
        }

        public string Id { get; }

        public string Sku { get; }

        public string CategoryCode { get; }
    }
}