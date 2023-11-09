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
        private readonly IProductService _product;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ICustomerService _customerService;
        public CustomerOrderController(ISession session):base(session)
        {
            _product = new IMS.Service.ProductService { Session = session };
            _orderHeaderService = new OrderHeaderService { Session = session };
            _orderDetailService = new OrderDetailService { Session = session };
            _customerService = new IMS.Service.CustomerService { Session = session };
        }
        // GET: Manager/CustomerOrder
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult Index( string status)
        {
            var orderHeader = _orderHeaderService.GetAllOrderHeaders();
            if (status == "Approved")
            {
                return View(orderHeader.Where(u=>u.OrderStatus=="Approved"));
            }else if(status == "Shipped")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "Shipped"));
            }
            else if (status == "InProcess")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "InProcess"));
            }
            else if (status == "Delivered")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "Delivered"));
            }
            else if (status == "Cancelled")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "Cancelled"));
            }
            else if (status == "All")
            {
                return View(orderHeader);
            }
            return View(orderHeader);
        }


        [Authorize(Roles ="Customer,Admin,Manager")]
        public ActionResult Edit(long id)
        {
            var orderHeader = _orderHeaderService.GetOrderHeaderById(id);
            var orderDetails = _orderDetailService.getAllOrderDetails().Where(u => u.OrderHeader.Id == orderHeader.Id);
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
                Email = customer.Email
            };
            return View(customerInvoiceViewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult UpdateOrderDetail(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            var orderHeader=_orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);
            orderHeader.Name= customerInvoiceViewModel.OrderHeader.Name;
            orderHeader.PhoneNumber= customerInvoiceViewModel.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress= customerInvoiceViewModel.OrderHeader.StreetAddress;
            orderHeader.City= customerInvoiceViewModel.OrderHeader.City;
            orderHeader.Thana=customerInvoiceViewModel.OrderHeader.Thana;
            orderHeader.PostalCode= customerInvoiceViewModel.OrderHeader.PostalCode;
            if (customerInvoiceViewModel.OrderHeader.Carrier != null)
            {
                orderHeader.Carrier= customerInvoiceViewModel.OrderHeader.Carrier;
            }
            if (customerInvoiceViewModel.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber= customerInvoiceViewModel.OrderHeader.TrackingNumber;
            }
            _orderHeaderService.Update(orderHeader);
            return RedirectToAction("Edit", "CustomerOrder", new {id=customerInvoiceViewModel.OrderHeader.Id});
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult StartProcessing(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            _orderHeaderService.UpdateStatus(customerInvoiceViewModel.OrderHeader.Id, ShoppingHelper.StatusInProces, customerInvoiceViewModel.OrderHeader.PaymentStatus);
            return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult ShipOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);
            orderHeader.TrackingNumber = Guid.NewGuid().ToString();
            orderHeader.Carrier = customerInvoiceViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = ShoppingHelper.StatusShipped;
            orderHeader.ShippingDate=DateTime.Now;
            _orderHeaderService.Update(orderHeader);
            return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public ActionResult DeliveredOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            _orderHeaderService.UpdateStatus(customerInvoiceViewModel.OrderHeader.Id, ShoppingHelper.StatusDelivered, customerInvoiceViewModel.OrderHeader.PaymentStatus);
            return RedirectToAction("Edit", "CustomerOrder", new { id = customerInvoiceViewModel.OrderHeader.Id });
        }
        [Authorize(Roles = "Manager,Admin,Customer")]
        [HttpPost]
        public ActionResult CancelOrder(CustomerInvoiceViewModel customerInvoiceViewModel)
        {
            var orderHeader = _orderHeaderService.GetOrderHeaderById(customerInvoiceViewModel.OrderHeader.Id);
            if(orderHeader.PaymentStatus==ShoppingHelper.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _orderHeaderService.UpdateStatus(orderHeader.Id, ShoppingHelper.StatusCancelled, ShoppingHelper.StatusRefunded);
                var orderDetails=_orderDetailService.getAllOrderDetails().Where(u=>u.OrderHeader.Id==orderHeader.Id).ToList();
                foreach (var item in orderDetails)
                {
                    var prod=_product.GetProductById(item.Product.Id);
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

    }
} 