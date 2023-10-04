using IMS.Models;
using IMS.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using(ISession session = NHibernateHelper.OpenSession())
            {
                var data= session.Query<Department>().ToList();
                return View(data);
            }
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
    }
}