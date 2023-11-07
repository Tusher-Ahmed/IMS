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
        private readonly IGarmentsService _garmentsService;
        
        public StaffHomeController(ISession session)
        {
            _product = new ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
            _garmentsService=new GarmentsService { Session = session };
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
            List<GarmentsProduct> garmentsProducts = new List<GarmentsProduct>();

            List<string> suppliersName = new List<string>();
            Dictionary<long,int> quantity= new Dictionary<long,int>();
            Dictionary<long,long> orderId= new Dictionary<long,long>();

            foreach(var item in prod)
            {                               
                long oId = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).OrderId;
                orderId.Add(item.OrderHistoryId, oId);
                int count = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).Quantity;
                quantity.Add(item.OrderHistoryId, count);
                var gProd = _garmentsService.GetGarmentsProductByProductCode(item.ProductCode);
                garmentsProducts.Add(gProd);
                string sup = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                suppliersName.Add(sup);
            }
            StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
            {
                Products=prod,
                GarmentsProducts= garmentsProducts,
                SuppliersName= suppliersName,
                Quantity= quantity,
                OrderIds= orderId,
            };
            
            return View(staffDashboardViewModel);
        }
    }
}