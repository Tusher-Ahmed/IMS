using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class ErrorController : Controller
    {

        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound(string returnUrl)
        {
            ViewBag.ReturnUrl=returnUrl;
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}