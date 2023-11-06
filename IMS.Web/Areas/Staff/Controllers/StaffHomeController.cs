using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
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
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        public StaffHomeController(ISession session)
        {
            _product = new ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
        }
        // GET: Staff/StaffHome
        public ActionResult Index()
        {
            StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
            {
                Products= _product.GetAllProduct().Where(u=>u.Approved==null ).ToList(),
                TotalNewProduct= _product.GetAllProduct().Where(u => u.Approved == null ).Count(),
                TotalApprovedProduct= _product.GetAllProduct().Where(u => u.Approved == true).Count(),               
            };
            return View(staffDashboardViewModel);
        }

       public ActionResult ApprovedProducts()
        {
            var prod = _product.GetAllProduct().Where(u => u.Approved == true).ToList();
            List<string> suppliersName = new List<string>();

            foreach(var item in prod)
            {
                string sup = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                suppliersName.Add(sup);
            }
            StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
            {
                Products = prod,
                SuppliersName= suppliersName
            };
            
            return View(staffDashboardViewModel);
        }
    }
}