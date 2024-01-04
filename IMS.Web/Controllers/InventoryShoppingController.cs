using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    
    public class InventoryShoppingController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGarmentsService _garmentsService;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        public InventoryShoppingController(ISession session):base(session)
        {
            _inventoryShoppingService=new InventoryShoppingService { Session = session };
            _garmentsService=new GarmentsService { Session=session};
            log4net.Config.XmlConfigurator.Configure();
        }
        // GET: InventoryShopping


        #region Product details with add to cart
        [Authorize(Roles = "Manager,Admin,Supplier")]
        public ActionResult ProductDetails(long ProductId,int count=1)
        {
            try
            {
                var garmentsProduct = _garmentsService.GetGarmentsProductById(ProductId);
                if (garmentsProduct != null)
                {
                    InventoryOrderCart inventoryShoppingCart = new InventoryOrderCart
                    {
                        GarmentsProduct = garmentsProduct,
                        ProductId = ProductId,
                        Count = count
                    };
                    return View(inventoryShoppingCart);
                }
                return RedirectToAction("Index", "Garments");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }


        }
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult ProductDetails( InventoryOrderCart inventoryOrderCart)
        {
            if (inventoryOrderCart.Count <= 0)
            {
                ModelState.AddModelError("Count", "Add at least 1 product.");
                return RedirectToAction("ProductDetails", new { ProductId = inventoryOrderCart.ProductId });
            }
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
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
                                ProductId = inventoryOrderCart.ProductId,
                            };

                            // Add the inventoryOrder to the shopping cart
                            _inventoryShoppingService.AddInventoryShoppingCart(inventoryOrder);
                            return RedirectToAction("Index", "Garments");
                        }
                    }

                    // Handle the case where the GarmentsProduct is not found or the model is invalid.
                    return View(inventoryOrderCart);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
                
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Inventory Cart
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult InventoryCart()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                InventoryCartViewModel inventoryCartViewModel = new InventoryCartViewModel
                {
                    OrderCarts = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == userId).ToList()
                };
                foreach (var cart in inventoryCartViewModel.OrderCarts)
                {
                    inventoryCartViewModel.TotalPrice += (cart.GarmentsProduct.Price * cart.Count);
                }

                return View(inventoryCartViewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Inventory Cart Operation
        [HttpPost]
        
        public JsonResult IncrementCount(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());

                var cart = _inventoryShoppingService.GetproductById(id, userId);
                if (cart != null)
                {
                    _inventoryShoppingService.IncrementCount(cart, 1);
                    var newTotalPrice = CalculateTotalPrice();
                    return Json(new { newCount = cart.Count, newTotalPrice });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { error = "Cart not found" });
                }
            }catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                log.Error("An error occurred in YourAction.", ex);
                return Json(new { message = "User ID mismatch" });
            }
            
        }
        [HttpPost]
        public JsonResult DecrementCount(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var cart = _inventoryShoppingService.GetproductById(id, userId);
                if (cart != null)
                {
                    _inventoryShoppingService.DecrementCount(cart, 1);
                    var newTotalPrice = CalculateTotalPrice();
                    return Json(new { newCount = cart.Count, newTotalPrice });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { error = "Cart not found" });
                }
                
            }catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                log.Error("An error occurred in YourAction.", ex);
                return Json(new { message = "User ID mismatch" });
            }
            
        }

        public ActionResult RemoveFromCart(long id)
        {
            
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var cart = _inventoryShoppingService.GetproductById(id, userId);
                if (cart != null)
                {
                    _inventoryShoppingService.RemoveProduct(cart);
                }
                return RedirectToAction("InventoryCart");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

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