using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoExpressions.Models;
using ExpressionsDemo;

namespace DemoExpressions.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult CustomerSearch()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult CustomerSearch(CustomerSearchModel model)
        {
            
            model.Results = CustomerCollection.GetCustomers(model);
            return View(model);
        }
    }
}