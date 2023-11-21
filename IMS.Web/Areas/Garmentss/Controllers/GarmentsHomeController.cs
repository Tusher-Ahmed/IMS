﻿using IMS.Models.ViewModel;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
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
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var history = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId);

                Dictionary<long, decimal> TotalPrice = new Dictionary<long, decimal>();

                foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                {
                    TotalPrice.Add(item.OrderId, TotalAmount(item.OrderId));
                }

                GarmentsDashboardViewModel viewModel = new GarmentsDashboardViewModel
                {
                    Products = _garmentsService.GetAllP(),
                    TotalProduct = _garmentsService.GetAllP().Where(u => u.GarmentsId == userId).Count(),
                    TotalHistory = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId).GroupBy(u => u.OrderId).Select(u => u.First()).Count(),
                    OrderHistory = history,
                    TotalPrice = TotalPrice,
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

        #region Order history
        public ActionResult OrderHistory()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var history = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId);

                Dictionary<long, decimal> TotalPrice = new Dictionary<long, decimal>();

                foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                {
                    TotalPrice.Add(item.OrderId, TotalAmount(item.OrderId));
                }

                GarmentsOrderHistoryViewModel garmentsOrderHistoryViewModel = new GarmentsOrderHistoryViewModel
                {
                    OrderHistory = history,
                    TotalPrice = TotalPrice,
                };

                return View(garmentsOrderHistoryViewModel);
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
                var HProd = _inventoryOrderHistoryService.GetByOrderId(orderId).Where(u => u.GarmentsId == Convert.ToInt64(User.Identity.GetUserId()));
                return View(HProd);
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

    }
}