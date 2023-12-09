using IMS.DataAccess.Mapping;
using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.Controllers;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Engine;
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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Customer Dashboard
        // GET: Customer/CustomerHome
        public ActionResult Index()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var orderHeaders = _orderHeaderService.GetOrderByStatus("All", userId);
                List<OrderDetail> orders = new List<OrderDetail>();
                foreach (var item in orderHeaders)
                {
                    var ODetail = _orderDetailService.GetOrderDetailByOrderHeaderId(item.Id);
                    orders.Add(ODetail);
                }

                //OrderDetails=_orderDetailService.getAllOrderDetails().
                CustomerDashboardViewModel viewModel = new CustomerDashboardViewModel
                {
                    OrderHeaders = orderHeaders,
                    OrderDetails = orders,
                    TotalOrders = _orderHeaderService.GetOrderByStatus("Delivered", userId).Count(),
                    NewArrival = orderHeaders.Where(u => u.OrderStatus != ShoppingHelper.StatusDelivered &&
                          u.OrderStatus != ShoppingHelper.StatusCancelled && u.OrderStatus != ShoppingHelper.StatusRefunded).Count(),
                    TotalCanceledOrder = _orderHeaderService.GetOrderByStatus("Refunded", userId).Count()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Order Details
        public ActionResult OrderDetails(string status)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                //var orderHeader = _orderHeaderService.GetAllOrderHeaders().Where(u => u.CustomerId == userId);
                var orderHeader = _orderHeaderService.GetOrderByStatus(status , userId);              
                return View(orderHeader);
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