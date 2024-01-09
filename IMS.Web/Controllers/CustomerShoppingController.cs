using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.Options;
using NHibernate;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Customer")]

    public class CustomerShoppingController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICustomerShoppingService _customerShopping;
        private readonly IProductService _productService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ICustomerService _customerService;
        public CustomerShoppingController(ISession session) : base(session)
        {
            _customerShopping = new CustomerShoppingService { Session = session };
            _productService = new IMS.Service.ProductService { Session = session };
            _orderHeaderService = new OrderHeaderService { Session = session };
            _orderDetailService = new OrderDetailService { Session = session };
            _customerService = new IMS.Service.CustomerService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }
        // GET: CustomerShopping

        #region Product Details with add to cart
        [AllowAnonymous]
        public ActionResult ProductDetails(long ProductId)
        {
            try
            {
                var product = _productService.GetProductById(ProductId);
                if (product != null)
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ProductDetails(ShoppingCart shoppingCart)
         {
            if (shoppingCart.Count <= 0)
            {
                ModelState.AddModelError("Count", "Add at least 1 product.");
                return RedirectToAction("ProductDetails", new {ProductId=shoppingCart.ProductId});
            }
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ProductDetails", "CustomerShopping", new { shoppingCart.ProductId }) });
                }

                if (ModelState.IsValid)
                {
                    // Retrieve the GarmentsProduct based on the ProductId from your service
                    var Product = _productService.GetProductById(shoppingCart.ProductId);

                    if (Product != null)
                    {
                        ShoppingCart customerOrder = new ShoppingCart
                        {
                            Count = shoppingCart.Count,
                            CustomerId = Convert.ToInt64(User.Identity.GetUserId()),
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Shopping Cart

        public ActionResult ShoppingCart()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var carts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == userId && u.Product.Status == 1).ToList();
                if(carts.Count > 0)
                {
                    CustomerShoppingCartViewModel shoppingCartViewModel = new CustomerShoppingCartViewModel
                    {
                        shoppingCarts = carts,
                        OrderHeader = new OrderHeader()
                    };

                    foreach (var cart in shoppingCartViewModel.shoppingCarts)
                    {
                        shoppingCartViewModel.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                    }


                    return View(shoppingCartViewModel);
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

        #region Order Summary with stripe
        [HttpGet]
        public ActionResult Summary()
        {
            try
            {
                var context = new ApplicationDbContext();
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var carts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == userId && u.Product.Status == 1).ToList();
                if(carts.Count > 0)
                {
                    var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                    var user = userManager.FindById(userId);
                    var customer = _customerService.GetCustomerByUserId(userId);

                    CustomerShoppingCartViewModel shoppingCartViewModel = new CustomerShoppingCartViewModel
                    {
                        shoppingCarts = carts,
                        OrderHeader = new OrderHeader()
                    };

                    foreach (var cart in shoppingCartViewModel.shoppingCarts)
                    {
                        shoppingCartViewModel.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                    }

                    shoppingCartViewModel.OrderHeader.Name = customer.Name;
                    shoppingCartViewModel.OrderHeader.PhoneNumber = user.PhoneNumber;
                    shoppingCartViewModel.OrderHeader.StreetAddress = customer.StreetAddress;
                    shoppingCartViewModel.OrderHeader.City = customer.City;
                    shoppingCartViewModel.OrderHeader.Thana = customer.Thana;
                    shoppingCartViewModel.OrderHeader.PostalCode = customer.PostalCode;

                    return View(shoppingCartViewModel);
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

        [HttpPost]
        public ActionResult Summary(CustomerShoppingCartViewModel customerShoppingCartViewModel)
        {
            try
            {
                var context = new ApplicationDbContext();
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var carts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == userId && u.Product.Status == 1).ToList();
                if(carts.Count > 0)
                {
                    var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                    var user = userManager.FindById(userId);
                    var customer = _customerService.GetCustomerByUserId(userId);

                    CustomerShoppingCartViewModel shoppingCartViewModel = new CustomerShoppingCartViewModel
                    {
                        shoppingCarts = carts,
                        OrderHeader = new OrderHeader()
                    };

                    foreach (var cart in shoppingCartViewModel.shoppingCarts)
                    {
                        shoppingCartViewModel.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                    }

                    shoppingCartViewModel.OrderHeader.Name = customerShoppingCartViewModel.OrderHeader.Name;
                    shoppingCartViewModel.OrderHeader.PhoneNumber = customerShoppingCartViewModel.OrderHeader.PhoneNumber;
                    shoppingCartViewModel.OrderHeader.StreetAddress = customerShoppingCartViewModel.OrderHeader.StreetAddress;
                    shoppingCartViewModel.OrderHeader.City = customerShoppingCartViewModel.OrderHeader.City;
                    shoppingCartViewModel.OrderHeader.Thana = customerShoppingCartViewModel.OrderHeader.Thana;
                    shoppingCartViewModel.OrderHeader.PostalCode = customerShoppingCartViewModel.OrderHeader.PostalCode;
                    shoppingCartViewModel.OrderHeader.PaymentStatus = ShoppingHelper.PaymentStatusPending;
                    shoppingCartViewModel.OrderHeader.OrderStatus = ShoppingHelper.StatusPending;
                    shoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
                    shoppingCartViewModel.OrderHeader.CustomerId = userId;
                    _orderHeaderService.AddOrderHeader(shoppingCartViewModel.OrderHeader);

                    foreach (var cart in shoppingCartViewModel.shoppingCarts)
                    {
                        OrderDetail orderDetail = new OrderDetail
                        {
                            ProductId = cart.Product.Id,
                            Product = cart.Product,
                            OrderHeaderId = shoppingCartViewModel.OrderHeader.Id,
                            OrderHeader = shoppingCartViewModel.OrderHeader,
                            Price = cart.Product.Price,
                            Count = cart.Count
                        };
                        _orderDetailService.Add(orderDetail);

                    }

                    var domain = "https://localhost:44369/";

                    var options = new SessionCreateOptions
                    {
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = domain + $"CustomerShopping/OrderConfirmation?id={shoppingCartViewModel.OrderHeader.Id}",
                        CancelUrl = domain + $"CustomerShopping/Summary",
                        PaymentMethodTypes = new List<string> { "card" },
                    };

                    foreach (var item in shoppingCartViewModel.shoppingCarts)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Product.Price * 100),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Name,
                                },
                            },
                            Quantity = item.Count,
                        };

                        options.LineItems.Add(sessionLineItem);
                    }

                    var service = new SessionService();
                    Session session = service.Create(options);
                    shoppingCartViewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;
                    shoppingCartViewModel.OrderHeader.SessionId = session.Id;
                    shoppingCartViewModel.OrderHeader.PaymentDate = DateTime.Now;
                    _orderHeaderService.Update(shoppingCartViewModel.OrderHeader);

                    return Redirect(session.Url);
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

        #region Order Confirmation with instant invoice
        public ActionResult OrderConfirmation(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                OrderHeader orderheader = _orderHeaderService.GetOrderHeaderByUser(id, userId);
                if(orderheader == null)
                {
                    return RedirectToAction("Index", "Error");
                }
                // var customer = _customerService.GetCustomerByUserId(userId);

                var service = new SessionService();
                Session session = service.Get(orderheader.SessionId);
                _orderHeaderService.UpdateStripeSessionAndIntent(orderheader.Id, session.Id, session.PaymentIntentId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _orderHeaderService.UpdateStatus(id, ShoppingHelper.StatusApproved, ShoppingHelper.PaymentStatusApproved);
                }

                List<ShoppingCart> shoppingCarts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == userId).ToList();
                foreach (var cart in shoppingCarts)
                {
                    _customerShopping.RemoveProduct(cart);
                }

                var OrderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == id);
                List<IMS.Models.Product> products = new List<IMS.Models.Product>();

                foreach (var orderDetail in OrderDetails)
                {
                    var product = _productService.GetProductById(orderDetail.ProductId);
                    products.Add(product);
                }

                foreach (var orderDetail in OrderDetails)
                {
                    var prod = _productService.GetProductById(orderDetail.ProductId);
                    prod.Quantity = prod.Quantity - orderDetail.Count;
                    _productService.UpdateProduct(prod);
                }

                CustomerInvoiceViewModel customerInvoiceViewModel = new CustomerInvoiceViewModel()
                {
                    OrderHeader = orderheader,
                    OrderDetails = OrderDetails,
                    Products = products
                };

                TempData["success"] = "Order Placed Successfully!";
                Session.Remove("CartItemCount");
                return View(customerInvoiceViewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Invoice for customer
        public ActionResult InvoiceForCustomer(long id)
        {
            
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                OrderHeader orderheader = _orderHeaderService.GetOrderHeaderByUser(id, userId);
                if (orderheader == null)
                {
                    return RedirectToAction("Index", "Error");
                }
                var OrderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == id && u.OrderHeader.CustomerId==userId );
                List<IMS.Models.Product> products = new List<IMS.Models.Product>();

                if(OrderDetails == null)
                {
                    return RedirectToAction("Index", "Error");
                }

                foreach (var orderDetail in OrderDetails)
                {
                    var product = _productService.GetProductById(orderDetail.ProductId);
                    products.Add(product);
                }

                CustomerInvoiceViewModel customerInvoiceViewModel = new CustomerInvoiceViewModel()
                {
                    OrderHeader = orderheader,
                    OrderDetails = OrderDetails,
                    Products = products
                };
                return View(customerInvoiceViewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }


        }
        #endregion

        #region Cart's basic operations

        [HttpPost]
        public JsonResult IncrementCount(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var cart = _customerShopping.GetById(id, userId);

                if (cart != null)
                {
                    _customerShopping.IncrementCount(cart, 1);
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
                log.Error("An error occurred in YourAction.", ex);
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { message = "User ID mismatch" });
            }
            
        }
        [HttpPost]
        public JsonResult DecrementCount(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var cart = _customerShopping.GetById(id, userId);

                if (cart != null)
                {
                    _customerShopping.DecrementCount(cart, 1);
                    var newTotalPrice = CalculateTotalPrice();
                    return Json(new { newCount = cart.Count, newTotalPrice });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { error = "Cart not found" });
                }
            }
            catch(Exception ex)
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
                var cart = _customerShopping.GetById(id, userId);

                if (cart != null)
                {
                    _customerShopping.RemoveProduct(cart);
                }

                return RedirectToAction("ShoppingCart");
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("ShoppingCart");
            }
        }
        private decimal CalculateTotalPrice()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var orderCarts = _customerShopping.GetAllOrders().Where(u => u.CustomerId == userId).ToList();
                decimal total = orderCarts.Sum(cart => cart.Product.Price * cart.Count);
                return total;
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return 0;
            }
            
        }
        #endregion
    }
}