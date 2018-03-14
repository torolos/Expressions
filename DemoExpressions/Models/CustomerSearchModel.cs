using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpressionsDemo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DemoExpressions.Models
{
    public class CustomerSearchModel
    {
        [DisplayName("Customer ID")]
        public string CustomerID { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Contact Name")]
        public string ContactName { get; set;}
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }

        public CustomerCollection Results { get; set; }
    }
}