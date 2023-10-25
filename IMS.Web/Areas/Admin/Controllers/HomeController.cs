using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.App_Start;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace IMS.Web.Areas.Admin.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly IProductService _product;
        private readonly IDepartmentService _department;
        private readonly IProductTypeService _productType;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IGarmentsService _garmentsService;
        private ApplicationUserManager _userManager;

        // GET: Admin/Home
        public HomeController(ISession session)
        {
            _product=new ProductService { Session = session };
            _department = new DepartmentService { Session = session };
            _productType = new ProductTypeService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _garmentsService = new GarmentsService { Session = session };
        }
        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region ProductList
        public ActionResult Index()
        {
           
            AdminDashboardViewModel prod = new AdminDashboardViewModel
            {
                Products = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true),
                TotalProduct= _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true).Count(),
                TotalEmployee= GetEmployeeWithRoles(),
                TotalShop= GetShopsWithRoles()

            };
            return View(prod);
        }
        #endregion

        #region Dashboard
        public ActionResult Dashboard()
        {
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Product List
        public ActionResult ProductList()
        {
            IEnumerable<Product> products = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true);
            return View(products);
        }
        #endregion

        #region Edit
        public ActionResult Edit(long id)
        {
            var prod = _product.GetProductById(id);
            if (prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("ManagePrice");
        }
        [HttpPost]
        public ActionResult Edit(long id, Product product)
        {
            var prod = _product.GetProductById(id);
            long userId = Convert.ToInt64(User.Identity.GetUserId()); 
            if (prod != null)
            {
                prod.Price = product.Price;
                prod.Name = product.Name;
                prod.Description = product.Description;
                prod.ModifyBy = userId;//ManagerId
                prod.Status = 1;
                prod.VersionNumber = prod.VersionNumber + 1;

                _product.UpdateProduct(prod);
                return RedirectToAction("ProductList");
            }
            return View(product);
        }
        #endregion

        #region Status
        public ActionResult Status(long id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            var prod = _product.GetProductById(id);
            if (prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public ActionResult Status(long id, Product product)
        {
            var prod = _product.GetProductById(id);
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            if (prod != null)
            {
                prod.Status = 0;
                prod.ModifyBy = userId;
                _product.UpdateProduct( prod);
                return RedirectToAction("ProductList");
            }
            return View();
        }
        #endregion

        #region Details
        public ActionResult Details(long id)
        {
            if (id != 0)
            {
                var prod = _product.GetProductById(id);
                return View(prod);
            }
            return RedirectToAction("ProductList");
        }
        #endregion

        #region Deactivated List
        public ActionResult DeactivatedList()
        {
            var prod=_product.GetAllProduct().Where(u=>u.Status == 0 && u.Approved==true && u.IsPriceAdded==true);
            if (prod != null)
            {
                return View(prod);

            }
            return RedirectToAction("ProductList");
        }
        #endregion

        #region Change Status
        public ActionResult ChangeStatus(long id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            var prod = _product.GetProductById(id);
            if (prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("ProductList");
        }
        [HttpPost]
        public ActionResult ChangeStatus(long id, Product product)
        {
            var prod = _product.GetProductById(id);
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            if (prod != null)
            {
                prod.Status = 1;
                prod.ModifyBy = userId;
                _product.UpdateProduct(prod);
                return RedirectToAction("ProductList");
            }
            return View();
        }
        #endregion

        #region EmployeeList
        public ActionResult TotalEmployee()
        {
            return View();
        }
        #endregion

        #region Get total employee and store
        public int GetEmployeeWithRoles()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));

            // Define the role names you want to filter on.
            string[] roleNames = { "Manager", "Staff" }; // Replace with your actual role names.

            // Get a list of users who have any of the specified roles.
            // Get the role IDs for the specified role names.
            var roleIds = roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .ToList();

            // Get a list of users who have any of the specified roles.
            var usersWithRoles = context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                .ToList();

            return usersWithRoles.Count();
        }

        public int GetShopsWithRoles()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));

            // Define the role names you want to filter on.
            string roleName = "Customer" ; // Replace with your actual role names.

            // Get a list of users who have any of the specified roles.
            // Get the role IDs for the specified role names.
            var roleIds = roleManager.Roles
                .Where(r => roleName.Contains(r.Name))
                .Select(r => r.Id);
                

            // Get a list of users who have any of the specified roles.
            var usersWithRoles = context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                .ToList();

            return usersWithRoles.Count();
        }
        #endregion

        #region Employee
        public ActionResult Employee()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));
            string[] roleNames = { "Manager", "Staff" }; 
            var roleIds = roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .ToList();

            var usersWithDetails = context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                .ToList();
            List<UserDetailViewModel> usersDetails=new List<UserDetailViewModel>();
            foreach(var user in usersWithDetails)
            {
                var userDetails = new UserDetailViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    City = user.City,
                    StreetAddress = user.StreetAddress,
                    Thana = user.Thana,
                    PostalCode = user.PostalCode,
                    ERole= userManager.GetRoles(user.Id).FirstOrDefault()
                };
                usersDetails.Add(userDetails);
            }

            return View(usersDetails);
        }
        #endregion

        #region Shop
        public ActionResult Shop()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));
            string roleNames = "Customer";
            var roleIds = roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .ToList();

            var usersWithDetails = context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                .ToList();
            List<UserDetailViewModel> usersDetails = new List<UserDetailViewModel>();
            foreach (var user in usersWithDetails)
            {
                var userDetails = new UserDetailViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    ShopName=user.ShopName,
                    City = user.City,
                    StreetAddress = user.StreetAddress,
                    Thana = user.Thana,
                    PostalCode = user.PostalCode,
                };
                usersDetails.Add(userDetails);
            }

            return View(usersDetails);
        }
        #endregion

        #region invoice
        public ActionResult InventoryOrder()
        {
            var history = _inventoryOrderHistoryService.GetAll().GroupBy(u=>u.OrderId).Select(u=>u.First());
            return View(history);
        }
        public ActionResult Invoice(long orderId)
        {
            var context = new ApplicationDbContext();
            
            var History = _inventoryOrderHistoryService.GetByOrderId(orderId);
            var firstOrderHistory = History.FirstOrDefault(); 

            if (firstOrderHistory != null)
            {
                var employeeId = firstOrderHistory.EmployeeId;
                var GarmentsId=firstOrderHistory.GarmentsId;
                var manager = context.Users.FirstOrDefault(u => u.Id == employeeId);
                var garments= context.Users.FirstOrDefault(u => u.Id == GarmentsId);
                if (manager != null && garments!=null)
                {
                    InventoryInvoiceViewModel inventoryInvoiceViewModel = new InventoryInvoiceViewModel
                    {
                        orderHistories = History,
                        GarmentsName = garments.GarmentsName,
                        ManagerEmail = manager.Email,
                    };
                    return View(inventoryInvoiceViewModel);
                }
            }
            
            return RedirectToAction("Index","Home");
        }
        #endregion

        public ActionResult ProductDetails(long orderId)
        {
            var context = new ApplicationDbContext();

            var History = _inventoryOrderHistoryService.GetByOrderId(orderId);
            var firstOrderHistory = History.FirstOrDefault();

            if (firstOrderHistory != null)
            {
                var employeeId = firstOrderHistory.EmployeeId;
                var GarmentsId = firstOrderHistory.GarmentsId;
                var manager = context.Users.FirstOrDefault(u => u.Id == employeeId);
                var garments = context.Users.FirstOrDefault(u => u.Id == GarmentsId);
                if (manager != null && garments != null)
                {
                    InventoryInvoiceViewModel inventoryInvoiceViewModel = new InventoryInvoiceViewModel
                    {
                        orderHistories = History,
                        GarmentsName = garments.GarmentsName,
                        ManagerEmail = manager.Email,
                    };
                    return View(inventoryInvoiceViewModel);
                }
            }
            return RedirectToAction("InventoryOrder", "Home");
        }
    }
}