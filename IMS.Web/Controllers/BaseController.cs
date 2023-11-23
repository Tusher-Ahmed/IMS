using IMS.Service;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            int cartItemCount = _customerShopping.GetAllOrders().Where(u => u.CustomerId == Convert.ToInt64(User.Identity.GetUserId()) && u.Product.Status==1).Count(); 
            HttpContext.Session["CartItemCount"] = cartItemCount;
            
            int InventoryCartCount = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == Convert.ToInt64(User.Identity.GetUserId())).Count();
            HttpContext.Session["InventoryCartItemCount"] = InventoryCartCount;

            int NeedApprovalCount= _productService.GetAllProduct().Where(u => u.Approved == null).Count();
            ViewBag.NeedApproval = NeedApprovalCount;

            int productShortageCount = _productService.GetAllProduct().Where(u => u.Quantity <= 0 && u.IsPriceAdded == true).Count();
            ViewBag.ProductShortageCount= productShortageCount;

        }
    }
}
