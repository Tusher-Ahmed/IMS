using System.Web.Mvc;

namespace IMS.Web.Areas.Garmentss
{
    public class GarmentssAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Garmentss";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Garmentss_default",
                "Garmentss/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}