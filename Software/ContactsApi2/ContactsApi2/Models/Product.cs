using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsApi2.Models
{
    public class Product
    {
        public string id { get; set; }
        /// <summary>
        /// Name of the product
        /// </summary>
        public string name { get; set; }
        public decimal price { get; set; }
    }
}