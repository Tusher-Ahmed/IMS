using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Utility;
using IMS.Web.App_Start;
using IMS.Web.Controllers;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using NHibernate;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace IMS.Web.Areas.Manager.Controllers
{

    public class ManagerHomeController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        private readonly IOrderHeaderService _orderHeaderService;
        private readonly IGarmentsService _garmentsService;
        private readonly IManageProductService _manageProductService;

        public ManagerHomeController(ISession session) : base(session)
        {
            _product = new IMS.Service.ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService = new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
            _orderHeaderService = new OrderHeaderService { Session = session };
            _garmentsService = new GarmentsService { Session = session };
            _manageProductService = new ManageProductService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Manager Dashboard
        // GET: Manager/ManagerHome
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            try
            {
                ManagerDashboardViewModel managerDashboardView = new ManagerDashboardViewModel
                {
                    TotalProducts = _product.GetAllApprovedProduct().Count(),
                    TotalShortage = _product.GetAllShortageProduct().Count(),
                    NewArrival = _product.GetAllNewProduct().Count(),
                    OrderHeaders = _orderHeaderService.GetAllOrderHeaders().OrderByDescending(u => u.Id).ToList(),
                    NewOrder = _orderHeaderService.GetOrderByStatus("Approved").Count(),
                    Processing = _orderHeaderService.GetOrderByStatus("InProcess").Count(),
                    TotalCancel = _orderHeaderService.GetOrderByStatus("Cancelled").Count(),
                    TotalShipped = _orderHeaderService.GetOrderByStatus("Shipped").Count(),
                    TotalRefunded = _orderHeaderService.GetAllOrderHeadersWithCondition("Cancelled", "Refunded").Count(),

                };

                return View(managerDashboardView);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Product List
        [Authorize(Roles = "Manager")]
        public ActionResult ProductList()
        {
            try
            {
                // var product = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true);
                if (User.IsInRole("Manager"))
                {
                    var product = _product.GetAllApprovedProduct();
                    return View(product);
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

        #region Edit
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(long id)
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var prod = _product.GetProductById(id);
                    if (prod != null)
                    {
                        return View(prod);
                    }
                    return RedirectToAction("ProductList");
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
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Edit(long id, IMS.Models.Product product, HttpPostedFileBase ImageFile)
        {
            
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var prod = _product.GetProductById(id);

                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    if (prod != null)
                    {
                        prod.Price = product.Price;
                        prod.Name = product.Name;

                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {

                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                            var path = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                            ImageFile.SaveAs(path);


                            prod.Image = uniqueFileName;
                        }
                        prod.Description = product.Description;
                        prod.ModifyBy = userId;//ManagerId
                        prod.Status = 1;
                        prod.VersionNumber = prod.VersionNumber + 1;

                        _product.UpdateProduct(prod);
                        TempData["success"] = "Product updated successfully!";
                        return RedirectToAction("ProductList");
                    }
                    return View(product);
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

        #region Status
        [Authorize(Roles = "Manager")]
        public ActionResult Status(long id)
        {
            try
            {
                if (User.IsInRole("Manager"))
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

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Status(long id, IMS.Models.Product product)
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var prod = _product.GetProductById(id);
                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    if (prod != null)
                    {
                        prod.Status = 0;
                        prod.ModifyBy = userId;
                        _product.UpdateProduct(prod);
                        TempData["success"] = "Product status change successfully!";
                        return RedirectToAction("ProductList");
                    }
                    return View();
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

        #region Details
        [Authorize(Roles = "Manager")]
        public ActionResult Details(long id)
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    if (id != 0)
                    {
                        var prod = _product.GetProductById(id);
                        return View(prod);
                    }
                    return RedirectToAction("ProductList");
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

        #region Deactivated List
        [Authorize(Roles = "Manager")]
        public ActionResult DeactivatedList()
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var prod = _product.LoadDeactivatedProducts();
                    if (prod != null)
                    {
                        return View(prod);

                    }
                    return RedirectToAction("ProductList");
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

        #region Change Status
        [Authorize(Roles = "Manager")]
        public ActionResult ChangeStatus(long id)
        {
            try
            {
                if (User.IsInRole("Manager"))
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
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult ChangeStatus(long id, IMS.Models.Product product)
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var prod = _product.GetProductById(id);
                    long userId = Convert.ToInt64(User.Identity.GetUserId());
                    if (prod != null)
                    {
                        prod.Status = 1;
                        prod.ModifyBy = userId;
                        _product.UpdateProduct(prod);
                        TempData["success"] = "Product status change successfully!";
                        return RedirectToAction("ProductList");
                    }
                    return View();
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

        #region get staff details
        [Authorize(Roles = "Manager")]
        public ActionResult TotalStaff()
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var context = new ApplicationDbContext();
                    var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
                    var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));
                    string roleNames = "Staff";
                    var roleIds = roleManager.Roles
                        .Where(r => roleNames.Contains(r.Name))
                        .Select(r => r.Id)
                        .ToList();

                    var usersWithDetails = context.Users
                        .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                        .ToList();
                    List<UserDetailViewModel> staffs = new List<UserDetailViewModel>();
                    foreach (var user in usersWithDetails)
                    {
                        var employee = _employeeService.GetEmployeeByUserId(user.Id);
                        if (employee.Status != 0)
                        {
                            UserDetailViewModel users = new UserDetailViewModel
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
                            staffs.Add(users);
                        }
                    }
                    return View(staffs);
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

        #region Inventory Order History
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult InventoryOrder()
        {
            return View();
        }
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult InventoryOrderDataTable(int draw, int start, int length, DateTime? startDate, DateTime? finalDate, string search)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    int recordsTotal = 0;
                    int recordsFiltered = 0;
                    var data = new List<object>();
                    var history = _inventoryOrderHistoryService.GetAll()
                           .GroupBy(u => u.OrderId)
                           .Select(u => u.First())
                           .ToList();
                    if (startDate != null && finalDate != null)
                    {
                        history = history.Where(x => x.CreationDate >= startDate && x.CreationDate <= finalDate.Value.AddDays(1)).ToList();
                    }
                    if (startDate != null)
                    {
                        history = history.Where(x => x.CreationDate >= startDate).ToList();
                    }

                    if (finalDate != null)
                    {
                        history = history.Where(x => x.CreationDate <= finalDate.Value.AddDays(1)).ToList();
                    }
                    if (!string.IsNullOrEmpty(search))
                    {
                        string res = RemoveLeadingZeros(search);
                        history = history.Where(x => x.OrderId.ToString().Contains(res) || GetEmployeeNameByHistoryId(x.EmployeeId).Contains(res)).ToList();
                    }
                    recordsTotal = history.Count;
                    recordsFiltered = recordsTotal;

                    history = history.OrderByDescending(u => u.OrderId).Skip(start).Take(length).ToList();



                    foreach (var item in history)
                    {
                        var context = new ApplicationDbContext();
                        string manager = context.Users.FirstOrDefault(u => u.Id == item.CreatedBy).Email;
                        var invoiceUrl = Url.Action("Invoice", "ManagerHome", new { area = "Manager", orderId = item.OrderId });
                        var productDetailsUrl = Url.Action("ProductDetails", "ManagerHome", new { area = "Manager", orderId = item.OrderId });

                        int depth = item.OrderId.ToString().Length;
                        int dif = 6 - depth;
                        string num = item.OrderId.ToString();

                        if (dif > 0)
                        {
                            string newId = new string('0', dif);
                            num = newId + num;
                        }

                        var str = new List<string>()
                {
                    $"{num}",
                    $"{manager}",
                    $"{item.CreationDate.ToString().AsDateTime().ToShortDateString()}",
                    $"{item.TotalPrice.ToString("C")}",
                    $@"<a href=""{invoiceUrl}"" class=""btn btn-outline-dark"">
                        <i class=""fa-solid fa-receipt""></i>
                    </a>
                    <a href=""{productDetailsUrl}"" class=""btn btn-outline-success"">
                        <i class=""fas fa-info-circle""></i>
                    </a>"
                };

                        data.Add(str);
                    }


                    return Json(new { draw, recordsTotal, recordsFiltered, start, length, data });
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

        private string GetEmployeeNameByHistoryId(long createdBy)
        {
            var context = new ApplicationDbContext();
            string manager = context.Users.FirstOrDefault(u => u.Id == createdBy).Email;
            return manager;
        }

        static string RemoveLeadingZeros(string input)
        {
            return input.TrimStart('0');
        }
        #endregion

        #region Rejected Order History
        public ActionResult RejectedOrder()
        {
            try
            {
                if (User.IsInRole("Manager"))
                {
                    var history = _product.GetAllProduct().Where(u => u.Approved == false && u.Rejected == true);
                    Dictionary<long, string> managers = new Dictionary<long, string>();
                    Dictionary<long, string> staffs = new Dictionary<long, string>();
                    Dictionary<long, string> garments = new Dictionary<long, string>();
                    foreach (var item in history)
                    {
                        managers.Add(item.Id, GetUserEmailById(item.CreatedBy));
                        staffs.Add(item.Id, GetUserEmailById(item.ApprovedBy));
                        var gName = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                        garments.Add(item.Id, gName);
                    }
                    RejectedProductListViewModel rejectedProductListViewModel = new RejectedProductListViewModel
                    {
                        Products = history,
                        Managers = managers,
                        Staffs = staffs,
                        Garments = garments
                    };
                    return View(rejectedProductListViewModel);
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

        #region Invoice
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult Invoice(long orderId)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var context = new ApplicationDbContext();

                    var History = _inventoryOrderHistoryService.GetByOrderId(orderId);
                    var firstOrderHistory = History.FirstOrDefault();
                    List<string> name = new List<string>();
                    foreach (var item in History)
                    {
                        var garments = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                        name.Add(garments);
                    }
                    if (firstOrderHistory != null)
                    {
                        var employeeId = firstOrderHistory.EmployeeId;
                        var manager = context.Users.FirstOrDefault(u => u.Id == employeeId);

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

                    return RedirectToAction("Index", "Home");
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

        #region Order History Product Details
        [Authorize(Roles = "Manager,Admin")]

        public ActionResult ProductDetails(long orderId)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
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

        #region Selling Report

        [Authorize(Roles = "Manager,Admin")]
        public ActionResult SellingReport()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var startDate = DateTime.Now.AddDays(-7);
                    var endDate = DateTime.Now;
                    var orderHeader = _orderHeaderService.GetSellingReports(startDate, endDate);

                    ViewBag.StartDateValue = startDate.ToString("yyyy-MM-dd");
                    ViewBag.EndDateValue = endDate.ToString("yyyy-MM-dd");

                    return View(orderHeader);
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

        public ActionResult LoadSellingRecord(DateTime? startDate, DateTime? endDate, string searchText)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    List<OrderHeader> orders = new List<OrderHeader>();
                    orders = _orderHeaderService.GetSellingReports(startDate, endDate, searchText);

                    return PartialView("Partial/_SellingReport", orders);
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

        #region Buying Report

        [Authorize(Roles = "Manager,Admin")]
        public ActionResult BuyingReport()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var context = new ApplicationDbContext();
                    var product = _product.GetAllProduct();
                    List<OrderHistory> history = new List<OrderHistory>();
                    List<IMS.Models.Product> returnHistory = new List<IMS.Models.Product>();

                    Dictionary<long, string> managers = new Dictionary<long, string>();
                    var startDate = DateTime.Now.AddDays(-7);
                    var endDate = DateTime.Now;

                    history = _inventoryOrderHistoryService.GetHistories(product.Select(x => x.OrderHistoryId).ToList(), startDate, endDate);
                    returnHistory = _product.GetRejectHistory(startDate, endDate);

                    foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                    {
                        string manager = context.Users.FirstOrDefault(u => u.Id == item.CreatedBy).Email;
                        managers.Add(item.OrderId, manager);
                    }

                    BuyingReportViewModel viewModel = new BuyingReportViewModel
                    {
                        History = history,
                        Name = managers,
                        RejectProducts = returnHistory
                    };

                    ViewBag.StartDateValue = startDate.ToString("yyyy-MM-dd");
                    ViewBag.EndDateValue = endDate.ToString("yyyy-MM-dd");

                    return View(viewModel);
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

        public ActionResult LoadReports(DateTime? startDate, DateTime? endDate, string searchText)
        {
            var context = new ApplicationDbContext();
            var product = _product.GetAllProduct().Where(u => u.Rejected != true);
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    List<OrderHistory> history = new List<OrderHistory>();
                    List<IMS.Models.Product> returnHistory = new List<IMS.Models.Product>();
                    history = _inventoryOrderHistoryService.GetHistories(product.Select(x => x.OrderHistoryId).ToList(), startDate, endDate, searchText);
                    returnHistory = _product.GetRejectHistory(startDate, endDate);
                    Dictionary<long, string> managers = new Dictionary<long, string>();

                    foreach (var item in history.GroupBy(u => u.OrderId).Select(t => t.First()))
                    {
                        string manager = context.Users.FirstOrDefault(u => u.Id == item.CreatedBy).Email;
                        managers.Add(item.OrderId, manager);
                    }

                    BuyingReportViewModel viewModel = new BuyingReportViewModel
                    {
                        History = history,
                        Name = managers,
                        RejectProducts = returnHistory
                    };

                    return PartialView("Partial/_OrderHistory", viewModel);
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

        #region Product Shortage
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult ProductShortage()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var products = _product.GetAllShortageProduct();
                    List<int> shortage = new List<int>();
                    List<long> productIds = new List<long>();
                    Dictionary<long, bool> IsInOrder = new Dictionary<long, bool>();
                    foreach (var product in products)
                    {
                        int count = 0 - product.Quantity;
                        long prodId = _garmentsService.GetGarmentsProductByProductCode(product.ProductCode).Id;
                        bool IsItInOrder = _product.GetAllProductByProductCode(product.ProductCode);
                        IsInOrder.Add(prodId, IsItInOrder);
                        productIds.Add(prodId);
                        shortage.Add(count);
                    }
                    ProductShortageViewModel productShortageViewModel = new ProductShortageViewModel
                    {
                        products = products,
                        ShortageCounts = shortage,
                        ProductIds = productIds,
                        IsInOrders = IsInOrder,
                    };
                    return View(productShortageViewModel);
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
    }
}