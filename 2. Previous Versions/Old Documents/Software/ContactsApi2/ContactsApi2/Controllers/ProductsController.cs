using ContactsApi2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ContactsApi2.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        /// <summary>
        /// Looks up some data by ID.
        /// </summary>
        /// <param name="id">The ID of the data.</param>
        [HttpGet]
        [Route("")]
        public IEnumerable<Product> Products()
        {
            var products = new List<Product>();
            products.Add(new Product()
            {
                name = "Green socks",
                price = 23
            });
            products.Add(new Product()
            {
                name = "Red socks",
                price = 12
            });
            return products;
        }

        /// <summary>
        /// jhjhg jhg jhg jhg hj
        /// </summary>
        /// <param name="id">kjhkhkjh</param>
        /// <returns></returns>
        [HttpGet]
        [MyBasicAuthenticationFilter]
        [Route("{id}")]
        public async Task<Product> Product(string id)
        {
            var product = new Product()
            {
                id = id,
                name = "Product " + id,
                price = 23
            };

            return product;
        }

        [HttpGet]
        [Route("{id}/abc")]
        public async Task<Product> abc(string id)
        {
            var product = new Product()
            {
                id = id,
                name = "Product " + id,
                price = 23
            };

            return product;
        }

    }
}
