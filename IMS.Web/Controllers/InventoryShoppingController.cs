using IMS.Models;
using IMS.Models.ViewModel;
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
    [Authorize(Roles = "Manager,Admin")]
    public class InventoryShoppingController : Controller
    {
        private readonly IGarmentsService _garmentsService;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        public InventoryShoppingController(ISession session)
        {
            _inventoryShoppingService=new InventoryShoppingService { Session = session };
            _garmentsService=new GarmentsService { Session=session};
        }
        // GET: InventoryShopping


        #region Product details with add to cart
        [AllowAnonymous]
        public ActionResult ProductDetails(long ProductId)
        {
            var garmentsProduct = _garmentsService.GetGarmentsProductById(ProductId);
            if(garmentsProduct != null)
            {
                InventoryOrderCart inventoryShoppingCart = new InventoryOrderCart
                {
                    GarmentsProduct = garmentsProduct,
                    ProductId = ProductId,
                    Count = 1
                };
                return View(inventoryShoppingCart);
            }
            return RedirectToAction("Index", "Garments");

        }
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult ProductDetails( InventoryOrderCart inventoryOrderCart)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the GarmentsProduct based on the ProductId from your service
                var garmentsProduct = _garmentsService.GetGarmentsProductById(inventoryOrderCart.ProductId);

                if (garmentsProduct != null)
                {
                    InventoryOrderCart inventoryOrder = new InventoryOrderCart
                    {
                        Count = inventoryOrderCart.Count,
                        EmployeeId = Convert.ToInt64(User.Identity.GetUserId()),
                        GarmentsId = garmentsProduct.GarmentsId,
                        GarmentsProduct = garmentsProduct,
                        ProductId=inventoryOrderCart.ProductId,
                    };

                    // Add the inventoryOrder to the shopping cart
                    _inventoryShoppingService.AddInventoryShoppingCart(inventoryOrder);                    
                    return RedirectToAction("Index", "Garments");
                }
            }

            // Handle the case where the GarmentsProduct is not found or the model is invalid.
            return View(inventoryOrderCart);
        }
        #endregion

        #region Inventory Cart
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult InventoryCart()
        {
            long userId= Convert.ToInt64(User.Identity.GetUserId());
            InventoryCartViewModel inventoryCartViewModel = new InventoryCartViewModel
            {
                OrderCarts = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == userId).ToList()
            };
            foreach(var cart in inventoryCartViewModel.OrderCarts)
            {
                inventoryCartViewModel.TotalPrice += (cart.GarmentsProduct.Price * cart.Count);
            }
           
            return View(inventoryCartViewModel);
        }
        #endregion

        #region Inventory Cart Operation
        [HttpPost]
        
        public JsonResult IncrementCount(long id)
        {
            var cart = _inventoryShoppingService.GetproductById(id);
            _inventoryShoppingService.IncrementCount(cart, 1);
            var newTotalPrice = CalculateTotalPrice();
            return Json(new { newCount = cart.Count, newTotalPrice }); ;
        }
        [HttpPost]
        public JsonResult DecrementCount(long id)
        {
            var cart = _inventoryShoppingService.GetproductById(id);
            _inventoryShoppingService.DecrementCount(cart, 1);
            var newTotalPrice = CalculateTotalPrice();
            return Json(new { newCount = cart.Count, newTotalPrice }); ;
        }

        public ActionResult RemoveFromCart(long id)
        {
            var cart = _inventoryShoppingService.GetproductById(id);
            if (cart != null)
            {
                _inventoryShoppingService.RemoveProduct(cart);
            } 
            return RedirectToAction("InventoryCart");
        }

        private decimal CalculateTotalPrice()
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            var orderCarts = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == userId).ToList();
            decimal total = orderCarts.Sum(cart => cart.GarmentsProduct.Price * cart.Count);
            return total;
        }

        #endregion
    }
}