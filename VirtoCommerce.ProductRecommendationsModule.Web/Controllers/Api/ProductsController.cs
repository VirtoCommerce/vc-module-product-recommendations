using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Controllers.Api
{
    [RoutePrefix("api/recomendations")]
    public class ProductsController : ApiController
    {
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(string[]))]
        public IHttpActionResult Get()
        {
            var result = new string[] {
                "9cbd8f316e254a679ba34a900fccb076",
                "f9330eb5ed78427abb4dc4089bc37d9f",
                "d154d30d76d548fb8505f5124d18c1f3",
                "1486f5a1a25f48a999189c081792a379",
                "ad4ae79ffdbc4c97959139a0c387c72e"
            };

            return Ok(result);
        }
    }
}
