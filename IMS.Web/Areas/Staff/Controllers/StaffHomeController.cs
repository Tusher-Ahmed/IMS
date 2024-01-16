using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.Controllers;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Staff.Controllers
{
    [Authorize(Roles ="Staff")]
    public class StaffHomeController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        private readonly IGarmentsService _garmentsService;
        
        public StaffHomeController(ISession session) : base(session)
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
           

            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                var user = userManager.FindById(userId);
                var status = _employeeService.GetEmployeeByUserId(userId).Status;
                if(User.IsInRole("Staff") && status != 0)
                {
                    var product = _product.LoadYetApprovedProduct();
                    Dictionary<long, string> garments = new Dictionary<long, string>();
                    foreach (var item in product)
                    {
                        string garment = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                        garments.Add(item.Id, garment);
                    }

                    StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
                    {
                        Products = product,
                        GName = garments,
                        TotalNewProduct = _product.LoadYetApprovedProduct().Count(),
                        TotalApprovedProduct = _product.LoadAllApprovedProducts().Count(),
                        TotalRejectedProduct = _product.LoadAllRejectedProducts().Count(),
                    };
                    return View(staffDashboardViewModel);
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

        #region Approved Product
        public ActionResult ApprovedProducts()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId); 

                if (role != null)
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
                        garments.Add(item.OrderHistoryId, sup);
                    }
                    StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
                    {
                        Products = prod,
                        GarmentsProducts = garmentsProducts,
                        GName = garments,
                        StaffName = staffs,
                        Quantity = quantity,
                        OrderIds = orderId,
                    };

                    return View(staffDashboardViewModel);
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
        public string GetUserEmailById(long? userId)
        {
            var context = new ApplicationDbContext();
            string manager = context.Users.FirstOrDefault(u => u.Id == userId).Email;

            return manager;
        }
        #endregion
    }
}