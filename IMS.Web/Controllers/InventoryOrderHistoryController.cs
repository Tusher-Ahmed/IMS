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
    public class InventoryOrderHistoryController : Controller
    {
        private readonly IInventoryOrderHistoryService _orderHistory;
        public InventoryOrderHistoryController(ISession session)
        {
            _orderHistory = new InventoryOrderHistoryService { Session = session };


        }
        // GET: InventoryOrderHistory
        public ActionResult Index(InventoryCartViewModel inventoryCartViewModel)
        {

            return View();
        }
    }
}