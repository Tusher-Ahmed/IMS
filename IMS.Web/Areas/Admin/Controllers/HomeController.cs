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
        private readonly IProductService _product;
        private readonly IDepartmentService _department;
        private readonly IProductTypeService _productType;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IGarmentsService _garmentsService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        private readonly ICustomerService _customerService;
        private readonly IOrderHeaderService _orderHeaderService;
        private ApplicationUserManager _userManager;

        // GET: Admin/Home
        public HomeController(ISession session):base(session)
        {
            _product = new ProductService { Session = session };
            _department = new DepartmentService { Session = session };
            _productType = new ProductTypeService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _garmentsService = new GarmentsService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
            _customerService = new CustomerService { Session = session };
            _orderHeaderService=new OrderHeaderService { Session = session };
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

            AdminDashboardViewModel prod = new AdminDashboardViewModel
            {
                Products = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true),
                TotalProduct = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true).Count(),
                TotalEmployee = GetEmployeeWithRoles(),
                TotalShop = GetShopsWithRoles(),
                orderHeaders = _orderHeaderService.GetAllOrderHeaders().OrderByDescending(u => u.Id).ToList(),
                TotalOrders =_orderHeaderService.GetAllOrderHeaders().Where(u=>u.OrderStatus==ShoppingHelper.StatusShipped || u.OrderStatus == ShoppingHelper.StatusDelivered).Count(),
                TotalNewOrders= _orderHeaderService.GetAllOrderHeaders().Where(u => u.OrderStatus != ShoppingHelper.StatusCancelled &&
                    u.OrderStatus != ShoppingHelper.StatusRefunded && u.OrderStatus != ShoppingHelper.StatusShipped).Count(),
                TotalCancelOrder= _orderHeaderService.GetAllOrderHeaders().Where(u => u.OrderStatus == ShoppingHelper.StatusCancelled && u.PaymentStatus==ShoppingHelper.StatusRefunded).Count()

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
            return RedirectToAction("ProductList");
        }
        [HttpPost]
        public ActionResult Edit(long id, Product product)
        {
            var prod = _product.GetProductById(id);
            var (processedDescription, primaryImageUrl) = ProcessDescription(product.Description);
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
                return RedirectToAction("ProductList");
            }
            return View(product);
        }
        private (string, string) ProcessDescription(string description)
        {

            string pattern = "<img.*?src=[\"'](.*?)[\"'].*?>";
            var match = Regex.Match(description, pattern);

            if (match.Success)
            {
                string dataUri = match.Groups[1].Value;

                if (dataUri.StartsWith("data:image/"))
                {
                    byte[] imageBytes = Convert.FromBase64String(dataUri.Split(',')[1]);

                    // Check the image size here
                    if (imageBytes.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Image", "Image size cannot exceed 5 MB.");
                        return (description, null);
                    }
                    string imageUrl = SaveDataUriAsImage(dataUri);

                    // Remove the embedded image from the description
                    string processedDescription = description.Replace(match.Value, string.Empty);

                    // Remove extra line breaks and white spaces
                    processedDescription = processedDescription.Trim();

                    return (processedDescription, imageUrl);
                }
            }

            // No embedded image found
            return (description, null);
        }

        private string SaveDataUriAsImage(string dataUri)
        {
            // Extract the file extension from the data URI
            string extension = dataUri.Split(';')[0].Split('/')[1];

            // Create a unique file name
            string fileName = Guid.NewGuid() + "." + extension;

            // Get the base64-encoded image data
            string base64Data = dataUri.Split(',')[1];

            // Decode and save the image as a file
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            string imagePath = Path.Combine(Server.MapPath("~/Images"), fileName); // Change this path to where you want to save the image
            System.IO.File.WriteAllBytes(imagePath, imageBytes);

            // Return the URL to the saved image
            return fileName; // Adjust the path as needed
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
                _product.UpdateProduct(prod);
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
            var prod = _product.GetAllProduct().Where(u => u.Status == 0 && u.Approved == true && u.IsPriceAdded == true);
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
            #region last effort
            //var context = new ApplicationDbContext();
            //var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            //var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));

            //// Define the role names you want to filter on.
            //string[] roleNames = { "Manager", "Staff" }; // Replace with your actual role names.

            //// Get a list of users who have any of the specified roles.
            //// Get the role IDs for the specified role names.
            //var roleIds = roleManager.Roles
            //    .Where(r => roleNames.Contains(r.Name))
            //    .Select(r => r.Id)
            //    .ToList();

            //// Get a list of users who have any of the specified roles.
            //var usersWithRoles = context.Users
            //    .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
            //    .ToList();

            //return usersWithRoles.Count();
            #endregion
            var employee = _employeeService.GetAllEmployee().Where(u=>u.Status==1).ToList();
            return employee.Count();
        }

        public int GetShopsWithRoles()
        {
            #region last effort
            //var context = new ApplicationDbContext();
            //var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            //var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));

            //string roleName = "Customer"; 
            //var roleIds = roleManager.Roles
            //    .Where(r => roleName.Contains(r.Name))
            //    .Select(r => r.Id);


            //// Get a list of users who have any of the specified roles.
            //var usersWithRoles = context.Users
            //    .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
            //    .ToList();

            //return usersWithRoles.Count();
            #endregion
            var shops = _customerService.GetAllCustomer().Where(u => u.Status == 1).ToList();
            return shops.Count();
        }
        #endregion

        #region Employee
        public ActionResult Employees()
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
                if(employee.Status != 0)
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
        #endregion

        #region Deactivate Employee
        public ActionResult DeactivateEmployee(long id)
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var user = userManager.FindById(id);
            var employee=_employeeService.GetEmployeeByUserId(user.Id);
            if (user != null && employee!=null)
            {                               
                employee.Status = 0;
                _employeeService.UpdateEmployee(employee);

                return RedirectToAction("Employees");
            }
            else
            {
                return RedirectToAction("Employees");
            }
            
        }
        #endregion

        #region Deactivate Employee List
        public ActionResult DeactivatedEmployeeList()
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
                var customer = _customerService.GetCustomerByUserId(user.Id);
                if(customer.Status != 0)
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
        #endregion

        #region Deactivate Customer/Shop
        public ActionResult DeactivateCustomer(long id)
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var user = userManager.FindById(id);
            var customer = _customerService.GetCustomerByUserId(user.Id);
            if (user != null && customer != null)
            {               
                customer.Status = 0;
                _customerService.UpdateCustomer(customer);

                return RedirectToAction("Shop");
            }
            else
            {
                return RedirectToAction("Shop");
            }
            
        }
        #endregion

        #region Deactivated Shop List
        public ActionResult DeactivatedCustomerList()
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
        #endregion
        #region Order History Product Details

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
                List<string> name = new List<string>();
                foreach (var item in History)
                {
                    var garments = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                    name.Add(garments);
                }
                if (manager != null )
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
        #endregion
    }
}