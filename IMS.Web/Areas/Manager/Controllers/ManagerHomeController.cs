﻿using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.App_Start;
using IMS.Web.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Manager.Controllers
{
    [Authorize(Roles ="Manager")]
    public class ManagerHomeController : Controller
    {
        private readonly IProductService _product;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ISupplierService _supplierService;
        public ManagerHomeController(ISession session)
        {
            _product = new ProductService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _employeeService=new EmployeeService { Session = session };
            _supplierService = new SupplierService { Session = session };
        }
        // GET: Manager/ManagerHome
        public ActionResult Index()
        {
            ManagerDashboardViewModel managerDashboardView = new ManagerDashboardViewModel
            {
                TotalProducts = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true).Count(),
                TotalStaff = GetAllStaff(),
                NewArrival= _product.GetAllProduct().Where(u => u.Approved == true && u.IsPriceAdded == false && u.Status == 0).Count()
            };
            
            return View(managerDashboardView);
        }
        public ActionResult ProductList()
        {
            var product = _product.GetAllProduct().Where(u => u.Status == 1 && u.IsPriceAdded == true && u.Approved == true);
            return View(product);
        }

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

        #region get Total staff (number)
        public int GetAllStaff()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser, long>(new UserStoreIntPk(context));
            var roleManager = new RoleManager<RoleIntPk, long>(new RoleStoreIntPk(context));
            string roleNames ="Staff";
            var roleIds = roleManager.Roles
                .Where(r => roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .ToList();

            var usersWithDetails = context.Users
                .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)))
                .ToList();
            List<Employee> staffs = new List<Employee>();
            foreach (var user in usersWithDetails)
            {
                var employee = _employeeService.GetEmployeeByUserId(user.Id);
                if (employee != null)
                {
                    staffs.Add(employee);

                }
            }
            return staffs.Count();
        }
        #endregion

        #region get staff details
        public ActionResult TotalStaff()
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
                if (employee != null)
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
        #endregion

        #region Inventory Order
        public ActionResult InventoryOrder()
        {
            //var history = _inventoryOrderHistoryService.GetAll().GroupBy(u => u.OrderId).Select(u => u.First());
            //return View(history);
            return View();
        }

        public ActionResult InventoryOrderDataTable(int draw, int start, int length, DateTime? startDate,DateTime? finalDate,string search)
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
                history = history.Where(x => x.CreationDate >= startDate && x.CreationDate <= finalDate).ToList();
            }
            if (startDate != null)
            {
                history = history.Where(x => x.CreationDate >= startDate).ToList();
            }

            if (finalDate != null)
            {
                history = history.Where(x => x.CreationDate <= finalDate).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                history = history.Where(x => x.OrderId.ToString().Contains(search) || GetGarmentsNameByHistoryId(x.GarmentsId).Contains(search)).ToList();
            }
            recordsTotal = history.Count;
            recordsFiltered = recordsTotal;

            history = history.Skip(start).Take(length).ToList();

           

            foreach (var item in history)
            {
                var GarmentsName = _supplierService.GetSupplierByUserId(item.GarmentsId).Name;
                var invoiceUrl = Url.Action("Invoice", "ManagerHome", new { area = "Manager", orderId = item.OrderId });
                var productDetailsUrl = Url.Action("ProductDetails", "ManagerHome", new { area = "Manager", orderId = item.OrderId });
                var str = new List<string>()
                {
                    $"{item.OrderId}",
                    $"{GarmentsName}",
                    $"{item.CreatedBy}",
                    $"{item.CreationDate}",
                    $"{item.TotalPrice}",
                    $@"<a href=""{invoiceUrl}"" class=""btn btn-outline-warning"">
                        <i class=""fa-regular fa-lightbulb""></i>
                    </a>
                    <a href=""{productDetailsUrl}"" class=""btn btn-outline-success"">
                        <i class=""fas fa-info-circle""></i>
                    </a>"
                };

                data.Add(str);
            }


            return Json(new {draw, recordsTotal, recordsFiltered, start, length, data});
        }
        private string GetGarmentsNameByHistoryId(long garmentsId)
        {
            return _supplierService.GetSupplierByUserId(garmentsId)?.Name;
        }
        #endregion

        #region Invoice
        public ActionResult Invoice(long orderId)
        {
            var context = new ApplicationDbContext();

            var History = _inventoryOrderHistoryService.GetByOrderId(orderId);
            var firstOrderHistory = History.FirstOrDefault();

            if (firstOrderHistory != null)
            {
                var employeeId = firstOrderHistory.EmployeeId;
                var GarmentsId = firstOrderHistory.GarmentsId;
                var manager = context.Users.FirstOrDefault(u => u.Id == employeeId);
                var garments = _supplierService.GetSupplierByUserId(GarmentsId).Name;
                if (manager != null && garments != null)
                {
                    InventoryInvoiceViewModel inventoryInvoiceViewModel = new InventoryInvoiceViewModel
                    {
                        orderHistories = History,
                        GarmentsName = garments,
                        ManagerEmail = manager.Email,
                    };
                    return View(inventoryInvoiceViewModel);
                }
            }

            return RedirectToAction("Index", "Home");
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
                var garments = _supplierService.GetSupplierByUserId(GarmentsId).Name;
                if (manager != null && garments != null)
                {
                    InventoryInvoiceViewModel inventoryInvoiceViewModel = new InventoryInvoiceViewModel
                    {
                        orderHistories = History,
                        GarmentsName = garments,
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