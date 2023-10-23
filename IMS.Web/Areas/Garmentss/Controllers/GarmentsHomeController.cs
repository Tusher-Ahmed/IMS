using IMS.Models.ViewModel;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Garmentss.Controllers
{
    public class GarmentsHomeController : Controller
    {
        private readonly IGarmentsService _garmentsService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        // GET: Garments/GarmentsHome
        public GarmentsHomeController(ISession session)
        {
            _garmentsService = new GarmentsService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
        }
        public ActionResult Index()
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            GarmentsDashboardViewModel viewModel = new GarmentsDashboardViewModel
            {
                Products = _garmentsService.GetAllP(),
                TotalProduct = _garmentsService.GetAllP().Where(u => u.GarmentsId == userId).Count(),
                TotalHistory = _inventoryOrderHistoryService.GetAll().Where(u => u.GarmentsId == userId).Count()
            };
            return View(viewModel);
        }
    }
}