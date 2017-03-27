using CsvHelper.Configuration;
using VirtoCommerce.ProductRecommendationsModule.Data.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvUserEventMap : CsvClassMap<UserEvent>
    {
        public CsvUserEventMap()
        {
            // https://westus.dev.cognitive.microsoft.com/docs/services/Recommendations.V4.0/operations/56f316efeda5650db055a3e2
            // User Id
            Map(x => x.UserId).Index(0);
            // Item Id
            Map(x => x.ItemId).Index(1);
            // Time
            Map(x => x.Created).Index(2);
            // Event
            Map(x => x.EventType).Index(3);
        }
    }
}