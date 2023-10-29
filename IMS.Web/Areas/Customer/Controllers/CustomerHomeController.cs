using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Customer.Controllers
{
    public class CustomerHomeController : Controller
    {
        public CustomerHomeController(ISession session)
        {
            
        }
        // GET: Customer/CustomerHome
        public ActionResult Index()
        {
            return View();
        }
    }
}