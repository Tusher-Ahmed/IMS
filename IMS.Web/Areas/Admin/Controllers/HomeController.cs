using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.App_Start;
using IMS.Web.Controllers;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;




namespace IMS.Web.Areas.Admin.Controllers
{
    
    [Authorize(Roles ="Admin")]
    public class HomeController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        private readonly ICustomerService _customerService;
        private readonly IOrderHeaderService _orderHeaderService;
        private ApplicationUserManager _userManager;
        private readonly IManageProductService _manageProductService;

        // GET: Admin/Home
        public HomeController(ISession session):base(session)
        {

            _product = new ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
            _customerService = new CustomerService { Session = session };
            _orderHeaderService=new OrderHeaderService { Session = session };
            _manageProductService = new ManageProductService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }
        public HomeController(ApplicationUserManager userManager, ISession session) : this(session)
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
        #region Dashboard View
        public ActionResult Index()
        {
            try
            {
                AdminDashboardViewModel prod = new AdminDashboardViewModel
                {
                    Products = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true),
                    TotalProduct = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true).Count(),
                    TotalEmployee = GetEmployeeWithRoles(),
                    TotalShop = GetShopsWithRoles(),
                    orderHeaders = _orderHeaderService.GetAllOrderHeaders().OrderByDescending(u => u.Id).ToList(),
                    TotalOrders = _orderHeaderService.GetAllOrderHeaders().Where(u => u.OrderStatus == ShoppingHelper.StatusShipped || u.OrderStatus == ShoppingHelper.StatusDelivered).Count(),
                    TotalNewOrders = _orderHeaderService.GetAllOrderHeaders().Where(u => u.OrderStatus != ShoppingHelper.StatusCancelled &&
                        u.OrderStatus != ShoppingHelper.StatusRefunded && u.OrderStatus != ShoppingHelper.StatusShipped).Count(),
                    TotalCancelOrder = _orderHeaderService.GetAllOrderHeaders().Where(u => u.OrderStatus == ShoppingHelper.StatusCancelled && u.PaymentStatus == ShoppingHelper.StatusRefunded).Count()

                };
                return View(prod);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
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
            try
            {
                IEnumerable<Product> products = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true);
                return View(products);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Edit
        public ActionResult Edit(long id)
        {
            try
            {
                var prod = _product.GetProductById(id);
                if (prod != null)
                {
                    return View(prod);
                }
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        public ActionResult Edit(long id, Product product)
        {
            try
            {
                var prod = _product.GetProductById(id);
                var targetFolderPath = Server.MapPath("~/Images");
                var (processedDescription, primaryImageUrl, error) = _manageProductService.ProcessDescription(product.Description, targetFolderPath);

                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("Image", error);
                }

                long userId = Convert.ToInt64(User.Identity.GetUserId());

                if (prod != null)
                {
                    prod.Price = product.Price;
                    prod.Name = product.Name;
                    prod.Description = processedDescription;

                    if (!string.IsNullOrEmpty(primaryImageUrl))
                    {
                        prod.Image = primaryImageUrl;
                    }
                    else
                    {
                        prod.Image = prod.Image;
                    }

                    prod.ModifyBy = userId;//ManagerId
                    prod.Status = 1;
                    prod.VersionNumber = prod.VersionNumber + 1;

                    _product.UpdateProduct(prod);
                    TempData["success"] = "Product Updated Successfully!";
                    return RedirectToAction("ProductList");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
       
        #endregion

        #region Status
        public ActionResult Status(long id)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

        [HttpPost]
        public ActionResult Status(long id, Product product)
        {
            try
            {
                var prod = _product.GetProductById(id);
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                if (prod != null)
                {
                    prod.Status = 0;
                    prod.ModifyBy = userId;
                    _product.UpdateProduct(prod);
                    TempData["success"] = "Status Updated Successfully!";
                    return RedirectToAction("ProductList");
                }
                return View();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Details
        public ActionResult Details(long id)
        {
            try
            {
                if (id != 0)
                {
                    var prod = _product.GetProductById(id);
                    return View(prod);
                }
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Deactivated List
        public ActionResult DeactivatedList()
        {
            try
            {
                var prod = _product.GetAllProduct().Where(u => u.Status == 0 && u.Approved == true && u.IsPriceAdded == true);
                if (prod != null)
                {
                    return View(prod);

                }
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Change Status
        public ActionResult ChangeStatus(long id)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }           
           
        }
        [HttpPost]
        public ActionResult ChangeStatus(long id, Product product)
        {
            try
            {
                var prod = _product.GetProductById(id);
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                if (prod != null)
                {
                    prod.Status = 1;
                    prod.ModifyBy = userId;
                    _product.UpdateProduct(prod);
                    TempData["success"] = "Status Updated Successfully!";
                    return RedirectToAction("ProductList");
                }
                return View();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
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
            var employee = _employeeService.GetAllEmployee().Where(u=>u.Status==1).ToList();
            return employee.Count();
        }

        public int GetShopsWithRoles()
        {         
            var shops = _customerService.GetAllCustomer().Where(u => u.Status == 1).ToList();
            return shops.Count();
        }
        #endregion

        #region Employee
        public ActionResult Employees()
        {
            try
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

                List<UserDetailViewModel> usersDetails = new List<UserDetailViewModel>();

                foreach (var user in usersWithDetails)
                {
                    var employee = _employeeService.GetEmployeeByUserId(user.Id);

                    if (employee.Status != 0)
                    {
                        UserDetailViewModel userDetails = new UserDetailViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Phone = user.PhoneNumber,
                            City = employee.City,
                            StreetAddress = employee.StreetAddress,
                            Thana = employee.Thana,
                            PostalCode = employee.PostalCode,
                            ERole = userManager.GetRoles(user.Id).FirstOrDefault()
                        };

                        usersDetails.Add(userDetails);
                    }
                }
                return View(usersDetails);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Deactivate Employee
        public ActionResult DeactivateEmployee(long id)
        {
            try
            {
                var context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                var user = userManager.FindById(id);
                var employee = _employeeService.GetEmployeeByUserId(user.Id);

                if (user != null && employee != null)
                {
                    employee.Status = 0;
                    _employeeService.UpdateEmployee(employee);
                    TempData["success"] = "Employee Deactivation Completed Successfully!";
                    return RedirectToAction("Employees");
                }
                else
                {
                    return RedirectToAction("Employees");
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
            
        }
        #endregion

        #region Deactivate Employee List
        public ActionResult DeactivatedEmployeeList()
        {
            try
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

                List<UserDetailViewModel> usersDetails = new List<UserDetailViewModel>();

                foreach (var user in usersWithDetails)
                {
                    var employee = _employeeService.GetEmployeeByUserId(user.Id);
                    if (employee.Status == 0)
                    {
                        UserDetailViewModel userDetails = new UserDetailViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Phone = user.PhoneNumber,
                            City = employee.City,
                            StreetAddress = employee.StreetAddress,
                            Thana = employee.Thana,
                            PostalCode = employee.PostalCode,
                            ERole = userManager.GetRoles(user.Id).FirstOrDefault()
                        };
                        usersDetails.Add(userDetails);
                    }
                }
                return View(usersDetails);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Shop
        public ActionResult Shop()
        {
            try
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
                    var customer = _customerService.GetCustomerByUserId(user.Id);
                    if (customer.Status != 0)
                    {
                        var userDetails = new UserDetailViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Phone = user.PhoneNumber,
                            ShopName = customer.Name,
                            City = customer.City,
                            StreetAddress = customer.StreetAddress,
                            Thana = customer.Thana,
                            PostalCode = customer.PostalCode,
                        };
                        usersDetails.Add(userDetails);
                    }
                }

                return View(usersDetails);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Deactivate Customer/Shop
        public ActionResult DeactivateCustomer(long id)
        {
            try
            {
                var context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                var user = userManager.FindById(id);
                var customer = _customerService.GetCustomerByUserId(user.Id);

                if (user != null && customer != null)
                {
                    customer.Status = 0;
                    _customerService.UpdateCustomer(customer);
                    TempData["success"] = "Customer Deactivated Successfully!";
                    return RedirectToAction("Shop");
                }
                else
                {
                    return RedirectToAction("Shop");
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
            
        }
        #endregion

        #region Deactivated Shop List
        public ActionResult DeactivatedCustomerList()
        {
            try
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
                    var customer = _customerService.GetCustomerByUserId(user.Id);
                    if (customer.Status == 0)
                    {
                        var userDetails = new UserDetailViewModel
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Phone = user.PhoneNumber,
                            ShopName = customer.Name,
                            City = customer.City,
                            StreetAddress = customer.StreetAddress,
                            Thana = customer.Thana,
                            PostalCode = customer.PostalCode,
                        };

                        usersDetails.Add(userDetails);
                    }
                }

                return View(usersDetails);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Order History Product Details

        public ActionResult ProductDetails(long orderId)
        {
            try
            {
                var context = new ApplicationDbContext();

                var History = _inventoryOrderHistoryService.GetByOrderId(orderId);
                var firstOrderHistory = History.FirstOrDefault();

                if (firstOrderHistory != null)
                {
                    var employeeId = firstOrderHistory.EmployeeId;
                    var GarmentsId = firstOrderHistory.GarmentsId;
                    var manager = context.Users.FirstOrDefault(u => u.Id == employeeId);
                    List<string> name = new List<string>();

                    foreach (var item in History)
                    {
                        var garments = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                        name.Add(garments);
                    }

                    if (manager != null)
                    {
                        InventoryInvoiceViewModel inventoryInvoiceViewModel = new InventoryInvoiceViewModel
                        {
                            orderHistories = History,
                            GarmentsName = name,
                            ManagerEmail = manager.Email,
                        };
                        return View(inventoryInvoiceViewModel);
                    }
                }

                return RedirectToAction("InventoryOrder", "Home");
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