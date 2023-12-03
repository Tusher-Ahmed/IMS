using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.Controllers;
using IMS.Web.Models;

using NHibernate;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Manager.Controllers
{
    
    public class CustomerOrderController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ICancelReasonService _cancelReasonService;
        public CustomerOrderController(ISession session):base(session)
        {
            _product = new IMS.Service.ProductService { Session = session };
            _orderHeaderService = new OrderHeaderService { Session = session };
            _orderDetailService = new OrderDetailService { Session = session };
            _cancelReasonService = new CancelReasonService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }
        // GET: Manager/CustomerOrder
        #region Customer Order List
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult Index( string status)
        {
            try
            {

                var orderHeader = _orderHeaderService.GetOrderByStatus(status);
                return View(orderHeader);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Edit
        [Authorize(Roles ="Customer,Admin,Manager")]
        public ActionResult Edit(long id)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(id);
                var orderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == orderHeader.Id);
                var cancleReason = _cancelReasonService.GetReasonByOrderHeaderId(orderHeader.Id);
                var context = new ApplicationDbContext();
                var customer = context.Users.FirstOrDefault(u => u.Id == orderHeader.CustomerId);

                List<IMS.Models.Product> products = new List<IMS.Models.Product>();

                foreach (var item in orderDetails)
                {
                    var prod = _product.GetProductById(item.Product.Id);
                    products.Add(prod);
                }

                CustomerInvoiceViewModel customerInvoiceViewModel = new CustomerInvoiceViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = orderDetails,
                    Products = products,
                    Email = customer.Email,
                    CancelReason = cancleReason,

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

        #region Update Order Detail
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult UpdateOrderDetail(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);
                orderHeader.Name = customerInvoiceViewModel.OrderHeader.Name;
                orderHeader.PhoneNumber = customerInvoiceViewModel.OrderHeader.PhoneNumber;
                orderHeader.StreetAddress = customerInvoiceViewModel.OrderHeader.StreetAddress;
                orderHeader.City = customerInvoiceViewModel.OrderHeader.City;
                orderHeader.Thana = customerInvoiceViewModel.OrderHeader.Thana;
                orderHeader.PostalCode = customerInvoiceViewModel.OrderHeader.PostalCode;

                if (customerInvoiceViewModel.OrderHeader.Carrier != null)
                {
                    orderHeader.Carrier = customerInvoiceViewModel.OrderHeader.Carrier;
                }

                if (customerInvoiceViewModel.OrderHeader.TrackingNumber != null)
                {
                    orderHeader.TrackingNumber = customerInvoiceViewModel.OrderHeader.TrackingNumber;
                }

                _orderHeaderService.Update(orderHeader);
                TempData["success"] = "Order Details Updated Successfully!";

                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Start Processing
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult StartProcessing(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                _orderHeaderService.UpdateStatus(customerInvoiceViewModel.OrderHeader.Id, ShoppingHelper.StatusInProces, customerInvoiceViewModel.OrderHeader.PaymentStatus);
                TempData["success"] = "Order status change successfully!";
                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Shipped Order
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult ShipOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);
                orderHeader.TrackingNumber = Guid.NewGuid().ToString();
                orderHeader.Carrier = customerInvoiceViewModel.OrderHeader.Carrier;
                orderHeader.OrderStatus = ShoppingHelper.StatusShipped;
                orderHeader.ShippingDate = DateTime.Now;
                _orderHeaderService.Update(orderHeader);
                TempData["success"] = "Order has been shipped successfully!";

                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Delivered Order
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult DeliveredOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                _orderHeaderService.UpdateStatus(customerInvoiceViewModel.OrderHeader.Id, ShoppingHelper.StatusDelivered, customerInvoiceViewModel.OrderHeader.PaymentStatus);
                TempData["success"] = "Order status change successfully!";
                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Cancel reason
        public ActionResult CancelReasonPage(long orderId)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(orderId);
                var orderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == orderHeader.Id);
                CustomerInvoiceViewModel customerInvoiceViewModel = new CustomerInvoiceViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = orderDetails,
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

        #region Cancel Order Without Refund by customer
        public ActionResult CancelOrderWithoutRefund(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);

                if (customerInvoiceViewModel.CancelReason.Reason == null)
                {
                    ModelState.AddModelError("cancel", "Cancel Reason is required");
                    return RedirectToAction("CancelReasonPage", new { orderId = customerInvoiceViewModel.OrderHeader.Id });
                }

                CancelReason cancelReason = new CancelReason
                {
                    OrderStatus = ShoppingHelper.StatusCancelled,
                    PaymentStatus = orderHeader.PaymentStatus,
                    OrderHeader = orderHeader,
                    Reason = customerInvoiceViewModel.CancelReason.Reason
                };

                _orderHeaderService.UpdateStatus(orderHeader.Id, ShoppingHelper.StatusCancelled, orderHeader.PaymentStatus);
                _cancelReasonService.AddReason(cancelReason);
                TempData["success"] = "Order Cancelled!";
                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Cancel/Return Order
        [Authorize(Roles = "Manager,Admin,Customer")]
        [HttpPost]
        public ActionResult CancelOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);

                if (orderHeader.PaymentStatus == ShoppingHelper.StatusApproved)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId
                    };

                    var service = new RefundService();
                    Refund refund = service.Create(options);
                    _orderHeaderService.UpdateStatus(orderHeader.Id, ShoppingHelper.StatusCancelled, ShoppingHelper.StatusRefunded);
                    TempData["success"] = "Payment Refunded Successfully!";
                    var orderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == orderHeader.Id).ToList();

                    foreach (var item in orderDetails)
                    {
                        var prod = _product.GetProductById(item.Product.Id);
                        prod.Quantity = prod.Quantity + item.Count;
                        _product.UpdateProduct(prod);
                    }
                }
                else
                {
                    _orderHeaderService.UpdateStatus(orderHeader.Id, ShoppingHelper.StatusCancelled, ShoppingHelper.StatusCancelled);
                }

                _orderHeaderService.Update(orderHeader);
                return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }            
        }
        #endregion

        #region Show Reason
        public ActionResult ShowReason(long id)
        {
            try
            {
                var orderHeader = _orderHeaderService.GetOrderHeaderById(id);
                var orderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == orderHeader.Id);
                var cancleReason = _cancelReasonService.GetReasonByOrderHeaderId(orderHeader.Id);

                CustomerInvoiceViewModel customerInvoiceViewModel = new CustomerInvoiceViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = orderDetails,
                    CancelReason = cancleReason
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
    }
} 