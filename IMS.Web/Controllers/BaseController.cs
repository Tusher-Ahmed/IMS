using IMS.Service;
using Microsoft.AspNet.Identity;
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
        public BaseController(ISession session)
        {
            _customerShopping = new CustomerShoppingService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService {  Session = session };
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Set the order count in the session and store it in ViewBag
            int cartItemCount = _customerShopping.GetAllOrders().Where(u => u.CustomerId == Convert.ToInt64(User.Identity.GetUserId())).Count(); // Implement your logic to get the cart item count
            ViewBag.CartItemCount = cartItemCount;

            int InventoryCartCount = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == Convert.ToInt64(User.Identity.GetUserId())).Count();
            ViewBag.InventoryCartItemCount= InventoryCartCount;
        }
    }
}
