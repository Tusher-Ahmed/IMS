using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Staff.Controllers
{
    [Authorize(Roles ="Staff")]
    public class StaffHomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        private readonly IGarmentsService _garmentsService;
        
        public StaffHomeController(ISession session)
        {
            _product = new ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
            _garmentsService=new GarmentsService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Staff Dashboard
        // GET: Staff/StaffHome
        public ActionResult Index()
        {
            var product = _product.GetAllProduct().Where(u => u.Approved == null).ToList();
            Dictionary<long, string> garments = new Dictionary<long, string>();
            foreach (var item in product)
            {
                string garment = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                garments.Add(item.Id, garment);
            }

            try
            {
                StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
                {
                    Products = product,
                    GName=garments,
                    TotalNewProduct = _product.GetAllProduct().Where(u => u.Approved == null).Count(),
                    TotalApprovedProduct = _product.GetAllProduct().Where(u => u.Approved == true).Count(),
                    TotalRejectedProduct = _product.GetAllProduct().Where(u => u.Approved == false && u.Rejected == true).Count(),
                };
                return View(staffDashboardViewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Approved Product
        public ActionResult ApprovedProducts()
        {
            try
            {
                var prod = _product.GetAllProduct().Where(u => u.Approved == true).ToList();
                List<GarmentsProduct> garmentsProducts = new List<GarmentsProduct>();

                Dictionary<long, string> garments = new Dictionary<long, string>();
                Dictionary<long, int> quantity = new Dictionary<long, int>();
                Dictionary<long, long> orderId = new Dictionary<long, long>();
                Dictionary<long, string> staffs = new Dictionary<long, string>();

                foreach (var item in prod)
                {
                    long oId = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).OrderId;
                    orderId.Add(item.OrderHistoryId, oId);

                    int count = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).Quantity;
                    quantity.Add(item.OrderHistoryId, count);

                    var gProd = _garmentsService.GetGarmentsProductByProductCode(item.ProductCode);
                    garmentsProducts.Add(gProd);

                    staffs.Add(item.OrderHistoryId, GetUserEmailById(item.ApprovedBy));

                    string sup = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                    garments.Add(item.OrderHistoryId,sup);
                }
                StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
                {
                    Products = prod,
                    GarmentsProducts = garmentsProducts,
                    GName= garments,
                    StaffName =staffs,
                    Quantity = quantity,
                    OrderIds = orderId,
                };

                return View(staffDashboardViewModel);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        public string GetUserEmailById(long? userId)
        {
            var context = new ApplicationDbContext();
            string manager = context.Users.FirstOrDefault(u => u.Id == userId).Email;

            return manager;
        }
        #endregion
    }
}