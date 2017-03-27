using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class CongnitiveRecommendationsServiceSettings
    {
        public string BaseUri { get; set; }

        public string ApiKey { get; set; }

        public string ModelId { get; set; }

        public string BuildId { get; set; }
    }
}
