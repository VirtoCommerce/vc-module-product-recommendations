using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.ProductRecommendationsModule.Data.Model
{
    public class SearchUserEventCriteria: SearchCriteriaBase
    {
        public string StoreId { get; set; }
    }
}
