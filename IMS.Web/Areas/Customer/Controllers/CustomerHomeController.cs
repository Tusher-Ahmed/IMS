using IMS.DataAccess.Mapping;
using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.Controllers;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Customer.Controllers
{
    [Authorize(Roles ="Customer")]
    
    public class CustomerHomeController : BaseController
    {
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly ICustomerShoppingService _customerShopping;

        public CustomerHomeController(ISession session):base(session) 
        {
            _orderHeaderService=new OrderHeaderService { Session = session };
            _orderDetailService=new OrderDetailService { Session=session};
            _customerService=new CustomerService { Session=session};
            _customerShopping=new CustomerShoppingService { Session=session};
           
        }
        // GET: Customer/CustomerHome
        public ActionResult Index()
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            var orderHeaders = _orderHeaderService.GetAllOrderHeaders().Where(u => u.CustomerId == userId);
            List<OrderDetail> orders = new List<OrderDetail>();
            foreach(var item in orderHeaders)
            {
                var ODetail = _orderDetailService.GetOrderDetailByOrderHeaderId(item.Id);
                orders.Add(ODetail);
            }

                //OrderDetails=_orderDetailService.getAllOrderDetails().
            CustomerDashboardViewModel viewModel = new CustomerDashboardViewModel
            {
              OrderHeaders=orderHeaders,
              OrderDetails=orders,
              TotalOrders=orderHeaders.Where(u => u.OrderStatus == ShoppingHelper.StatusShipped).Count(),
              NewArrival=orderHeaders.Where(u=>u.OrderStatus!=ShoppingHelper.StatusShipped &&
                    u.OrderStatus!=ShoppingHelper.StatusCancelled && u.OrderStatus != ShoppingHelper.StatusRefunded).Count(),
              TotalCanceledOrder= orderHeaders.Where(u => u.OrderStatus == ShoppingHelper.StatusCancelled).Count()
            };
            
            return View(viewModel);
        }

        public ActionResult OrderDetails(string status)
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            var orderHeader = _orderHeaderService.GetAllOrderHeaders().Where(u=>u.CustomerId==userId);
            if (status == "Approved")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "Approved"));
            }
            else if (status == "Shipped")
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
                return View(orderHeader.Where(u => u.OrderStatus == "Cancelled" && u.PaymentStatus != ShoppingHelper.StatusRefunded));
            }
            else if (status == "Refunded")
            {
                return View(orderHeader.Where(u => u.OrderStatus == "Cancelled" && u.PaymentStatus == ShoppingHelper.StatusRefunded));
            }
            else if (status == "All")
            {
                return View(orderHeader);
            }
            
            return View(orderHeader);

            
        }
    }
}