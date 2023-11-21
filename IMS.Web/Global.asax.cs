using IMS.Service;
using IMS.Web.ControllerFactory;
using NHibernate;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IMS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string stripePublishableKey = ConfigurationManager.AppSettings["StripePublishableKey"];
            string stripeSecretKey = ConfigurationManager.AppSettings["StripeSecretKey"];

            StripeConfiguration.ApiKey = stripeSecretKey;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();

            ISession session = NHibernateHelper.OpenSession();

            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(session));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            // Log the exception if needed

            // Handle 404 errors more gracefully
            if (exception is HttpException httpException && httpException.GetHttpCode() == 404)
            {
                Response.Clear();
                Server.ClearError();

                // Redirect to the custom 404 error page
                Response.Redirect("~/Error/NotFound");
            }
            
        }
    }
}
