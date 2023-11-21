using IMS.Web.Areas.Admin.Controllers;
using IMS.Web.Areas.Customer.Controllers;
using IMS.Web.Areas.Garmentss.Controllers;
using IMS.Web.Areas.Manager.Controllers;
using IMS.Web.Areas.Staff.Controllers;
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
                if (IsControllerTypeHandled(controllerType))
                {
                    if (controllerType == typeof(DepartmentController))
                    {
                        return new DepartmentController(_session);
                    }
                    else if (controllerType == typeof(ProductTypeController))
                    {
                        return new ProductTypeController(_session);
                    }
                    else if (controllerType == typeof(ProductController))
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
                    else if (controllerType == typeof(CustomerShoppingController))
                    {
                        return new CustomerShoppingController(_session);
                    }
                    else if (controllerType == typeof(HomeController))
                    {
                        return new HomeController(_session);
                    }
                    else if (controllerType == typeof(GarmentsHomeController))
                    {
                        return new GarmentsHomeController(_session);
                    }
                    else if (controllerType == typeof(ManagerHomeController))
                    {
                        return new ManagerHomeController(_session);
                    }
                    else if (controllerType == typeof(AccountController))
                    {
                        return new AccountController(_session);
                    }
                    else if (controllerType == typeof(StaffHomeController))
                    {
                        return new StaffHomeController(_session);
                    }
                    else if (controllerType == typeof(CustomerHomeController))
                    {
                        return new CustomerHomeController(_session);
                    }
                    else if (controllerType == typeof(CustomerOrderController))
                    {
                        return new CustomerOrderController(_session);
                    }
                    else if (controllerType == typeof(BaseController))
                    {
                        return new BaseController(_session);
                    }

                }
                return base.GetControllerInstance(requestContext, controllerType);
            }
            else
            {

                requestContext.HttpContext.Response.Redirect("~/Error/NotFound");
                return null;
            }
            
        }

        private bool IsControllerTypeHandled(Type controllerType)
        {
            List<Type> handledControllerTypes = new List<Type>
            {
                typeof(DepartmentController),
                typeof(ProductTypeController),
                typeof(ProductController),
                typeof(GarmentsController),
                typeof(InventoryShoppingController),
                typeof(InventoryOrderHistoryController),
                typeof(CustomerShoppingController),
                typeof(HomeController),
                typeof(GarmentsHomeController),
                typeof(ManagerHomeController),
                typeof(AccountController),
                typeof(StaffHomeController),
                typeof(CustomerHomeController),
                typeof(CustomerOrderController),
                typeof(BaseController),
                typeof(ManageController),
            };

            return handledControllerTypes.Contains(controllerType);
        }


    }
}