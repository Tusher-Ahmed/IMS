using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class CustomerShoppingController : Controller
    {
        private readonly ICustomerShoppingService _customerShopping;
        private readonly IProductService _productService;
        public CustomerShoppingController(ISession session)
        {
            _customerShopping=new CustomerShoppingService { Session=session };
            _productService=new ProductService { Session=session};
        }
        // GET: CustomerShopping

        #region Product Details with add to cart
        public ActionResult ProductDetails(long ProductId)
        {
            var product=_productService.GetProductById(ProductId);
            if(product != null)
            {
                ShoppingCart shoppingCart = new ShoppingCart
                {
                    Product = product,
                    ProductId = ProductId,
                    Count = 1,
                };
                return View(shoppingCart);
            }
            return RedirectToAction("Index", "Product");
        }
        [HttpPost]
        public ActionResult ProductDetails(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the GarmentsProduct based on the ProductId from your service
                var Product = _productService.GetProductById(shoppingCart.ProductId);

                if (Product != null)
                {
                    ShoppingCart customerOrder = new ShoppingCart
                    {
                        Count = shoppingCart.Count,
                        CustomerId = 1,
                        Product = Product,
                        ProductId = shoppingCart.ProductId,
                    };

                    // Add the inventoryOrder to the shopping cart
                    _customerShopping.AddCutomerShoppingCart(customerOrder);
                    return RedirectToAction("Index", "Product");
                }
            }

            // Handle the case where the GarmentsProduct is not found or the model is invalid.
            return View(shoppingCart);
        }
        #endregion

        #region Shopping Cart
        public ActionResult ShoppingCart()
        {
            CustomerShoppingCartViewModel shoppingCartViewModel = new CustomerShoppingCartViewModel
            {
                shoppingCarts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == 1).ToList()
            };
            foreach (var cart in shoppingCartViewModel.shoppingCarts)
            {
                shoppingCartViewModel.TotalPrice += (cart.Product.Price * cart.Count);
            }

            return View(shoppingCartViewModel);
        }
        #endregion
        [HttpPost]
        public JsonResult IncrementCount(long id)
        {
            var cart = _customerShopping.GetById(id);
            _customerShopping.IncrementCount(cart, 1);
            var newTotalPrice = CalculateTotalPrice();
            return Json(new { newCount = cart.Count, newTotalPrice }); ;
        }
        [HttpPost]
        public JsonResult DecrementCount(long id)
        {
            var cart = _customerShopping.GetById(id);
            _customerShopping.DecrementCount(cart, 1);
            var newTotalPrice = CalculateTotalPrice();
            return Json(new { newCount = cart.Count, newTotalPrice }); ;
        }

        public ActionResult RemoveFromCart(long id)
        {
            var cart = _customerShopping.GetById(id);
            if (cart != null)
            {
                _customerShopping.RemoveProduct(cart);
            }
            return RedirectToAction("InventoryCart");
        }
        private decimal CalculateTotalPrice()
        {
            var orderCarts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == 1).ToList();
            decimal total = orderCarts.Sum(cart => cart.Product.Price * cart.Count);
            return total;
        }
    }
}