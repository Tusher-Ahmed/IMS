using IMS.Service;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NHibernate;
using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Ast;

namespace IMS.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICustomerShoppingService _customerShopping;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        public BaseController(ISession session)
        {
            _customerShopping = new CustomerShoppingService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService {  Session = session };
            _productService = new ProductService {  Session = session };
            _customerService = new CustomerService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public string IsAuthorize(long userId)
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var user = userManager.FindById(userId);
            if (user != null)
            {
                string userRole = "";
                int status = 0;
                var roles = userManager.GetRoles(userId);
                if (roles.Count > 0)
                {
                    userRole = roles[0];
                }

                if (userRole == "Manager")
                {
                    status = _employeeService.GetEmployeeByUserId(userId).Status;
                }
                if(userRole == "Admin")
                {
                    status = 1;
                }

                if(status == 1)
                {
                    return userRole;
                }
            }
            return null;
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            #region check members status if not active logging them out and redirect them to login page
            if (User.Identity.IsAuthenticated)
            {
                var context = new ApplicationDbContext();
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                if (User.IsInRole("Manager") || User.IsInRole("Staff"))
                {
                    int status = _employeeService.GetEmployeeByUserId(userId).Status;
                    if (status == 0)
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        filterContext.Result = new RedirectResult("~/Account/Login");
                        return;
                    }
                }
                if (User.IsInRole("Customer"))
                {
                    int status = _customerService.GetCustomerByUserId(userId).Status;
                    if (status == 0)
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        filterContext.Result = new RedirectResult("~/Account/Login");
                        return;
                    }
                }
            }
            #endregion

            if (User.IsInRole("Customer"))
            {
                int cartItemCount = _customerShopping.GetAllCartOrders(Convert.ToInt64(User.Identity.GetUserId())).Where(u => u.Product.Status == 1).Count();
                HttpContext.Session["CartItemCount"] = cartItemCount;
            }
            
            if(User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                int InventoryCartCount = _inventoryShoppingService.LoadAllInventoryOrders(Convert.ToInt64(User.Identity.GetUserId())).Count();
                HttpContext.Session["InventoryCartItemCount"] = InventoryCartCount;

                int NeedApprovalCount = _productService.LoadYetApprovedProduct().Count();
                ViewBag.NeedApproval = NeedApprovalCount;

                int productShortageCount = _productService.GetAllShortageProduct().Count();
                ViewBag.ProductShortageCount = productShortageCount;

                int TotalInQueue = _productService.GetAllNewProduct().Count();
                ViewBag.SetPriceCount = TotalInQueue;
            }            
           
        }
        public string IsAuthorizeRole(long userId)
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var user = userManager.FindById(userId);
            if (user != null)
            {
                string userRole = "";
                int status = 0;
                var roles = userManager.GetRoles(userId);
                if (roles.Count > 0)
                {
                    userRole = roles[0];
                }

                if (userRole == "Manager" || userRole=="Staff")
                {
                    status = _employeeService.GetEmployeeByUserId(userId).Status;
                }
                if (userRole == "Admin")
                {
                    status = 1;
                }

                if (status == 1)
                {
                    return userRole;
                }
            }
            return null;
        }
    }
}
