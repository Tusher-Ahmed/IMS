using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Garmentss.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class GarmentsHomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGarmentsService _garmentsService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        // GET: Garments/GarmentsHome
        public GarmentsHomeController(ISession session)
        {
            _garmentsService = new GarmentsService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Garments Dashboard
        public ActionResult Index()
        {
            try
            {
                if (User.IsInRole("Supplier"))
                {
                    var context = new ApplicationDbContext();
                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    var history = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId);

                    Dictionary<long, decimal> TotalPrice = new Dictionary<long, decimal>();
                    Dictionary<long, string> orderBy = new Dictionary<long, string>();

                    string manager = string.Empty;
                    foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                    {
                        TotalPrice.Add(item.OrderId, TotalAmount(item.OrderId));
                        manager = context.Users.FirstOrDefault(u => u.Id == item.CreatedBy).Email;
                        orderBy.Add(item.OrderId, manager);
                    }

                    GarmentsDashboardViewModel viewModel = new GarmentsDashboardViewModel
                    {
                        Products = _garmentsService.GetAllP(),
                        TotalProduct = _garmentsService.GetAllP().Where(u => u.GarmentsId == userId).Count(),
                        TotalHistory = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId).GroupBy(u => u.OrderId).Select(u => u.First()).Count(),
                        OrderHistory = history,
                        TotalPrice = TotalPrice,
                        OrderBy = orderBy,
                    };

                    return View(viewModel);
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

        #region Order history
        public ActionResult OrderHistory()
        {
            try
            {
                if (User.IsInRole("Supplier"))
                {
                    var context = new ApplicationDbContext();
                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    var history = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId);

                    Dictionary<long, decimal> TotalPrice = new Dictionary<long, decimal>();
                    Dictionary<long, string> orderBy = new Dictionary<long, string>();

                    string manager = string.Empty;

                    foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                    {
                        TotalPrice.Add(item.OrderId, TotalAmount(item.OrderId));
                        manager = context.Users.FirstOrDefault(u => u.Id == item.CreatedBy).Email;
                        orderBy.Add(item.OrderId, manager);
                    }

                    GarmentsOrderHistoryViewModel garmentsOrderHistoryViewModel = new GarmentsOrderHistoryViewModel
                    {
                        OrderHistory = history,
                        TotalPrice = TotalPrice,
                        OrderBy = orderBy
                    };

                    return View(garmentsOrderHistoryViewModel);
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

        #region Order Details
        public ActionResult Details(long orderId)
        {
            try
            {
                if (User.IsInRole("Supplier"))
                {
                    var HProd = _inventoryOrderHistoryService.GetByOrderId(orderId).Where(u => u.GarmentsId == Convert.ToInt64(User.Identity.GetUserId()));
                    return View(HProd);
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

        #region Total Amount calculation
        public decimal TotalAmount(long orderId)
        {
            var HProd = _inventoryOrderHistoryService.GetByOrderId(orderId).Where(u => u.GarmentsId == Convert.ToInt64(User.Identity.GetUserId()));
            decimal price = 0;
            foreach (var item in HProd)
            {
                price += item.Price * item.Quantity;
            }
            return price;
        }
        #endregion

        #region Rejected Order
        public ActionResult RejectedOrder()
        {
            try
            {
                if (User.IsInRole("Supplier"))
                {
                    var context = new ApplicationDbContext();
                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    var products = _inventoryOrderHistoryService.GetAllRejectedOrder(userId);
                    Dictionary<long, string> orderBy = new Dictionary<long, string>();

                    foreach (var product in products)
                    {
                        string manager = context.Users.FirstOrDefault(u => u.Id == product.CreatedBy).Email;
                        orderBy.Add(product.OrderHistoryId, manager);
                    }

                    RejectedOrderViewModel rejectedOrderViewModel = new RejectedOrderViewModel
                    {
                        product = products,
                        OrderBy = orderBy,
                    };

                    return View(rejectedOrderViewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

    }
}