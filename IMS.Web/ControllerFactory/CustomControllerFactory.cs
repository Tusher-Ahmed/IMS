using IMS.Web.Controllers;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.ControllerFactory
{
    public class CustomControllerFactory: DefaultControllerFactory
    {
        private readonly ISession _session;

        public CustomControllerFactory(ISession session)
        {
            _session = session;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                if (controllerType == typeof(DepartmentController))
                {
                    return new DepartmentController(_session);
                }
                else if (controllerType == typeof(ProductTypeController))
                {
                    return new ProductTypeController(_session);
                }
                else if(controllerType == typeof(ProductController))
                {
                    return new ProductController(_session);
                }
                else if (controllerType == typeof(GarmentsController))
                {
                    return new GarmentsController(_session);
                }
                else if (controllerType == typeof(InventoryShoppingController))
                {
                    return new InventoryShoppingController(_session);
                }
                else if (controllerType == typeof(InventoryOrderHistoryController))
                {
                    return new InventoryOrderHistoryController(_session);
                }
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}