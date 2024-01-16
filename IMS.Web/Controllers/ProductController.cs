using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using IMS.Web.Models;
using log4net;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{

    public class ProductController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductService _product;
        private readonly IDepartmentService _department;
        private readonly IProductTypeService _productType;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IGarmentsService _garmentsService;
        private readonly ICustomerShoppingService _customerShopping;
        private readonly ISupplierService _supplier;
        public ProductController(ISession session) : base(session)
        {
            _product = new ProductService { Session = session };
            _department = new DepartmentService { Session = session };
            _productType = new ProductTypeService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService { Session = session };
            _inventoryOrderHistoryService = new InventoryOrderHistoryService { Session = session };
            _garmentsService = new GarmentsService { Session = session };
            _customerShopping = new CustomerShoppingService { Session = session };
            _supplier = new SupplierService { Session = session };
            log4net.Config.XmlConfigurator.Configure();

        }

        #region Product page
        // GET: Product
        [AllowAnonymous]
        public ActionResult Index(ProductViewModel product)
        {
            if (User.IsInRole("Supplier"))
            {
                return RedirectToAction("Index","Garments");
            }

            try
            {
                product = _product.GetProducts(product);
                product.ProductTypes = _productType.GetAllType().ToList();
                product.Departments = _department.GetAllDept().ToList();
                long userId = Convert.ToInt64(User.Identity.GetUserId());

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ProductListPartial", product);
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

        #region Product Details
        [AllowAnonymous]
        public ActionResult ProductDetails(long ProductId)
        {
            try
            {
                var product = _product.GetProductById(ProductId);
                if (product != null)
                {
                    return View(product);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Inventory Place Order and Add product
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult PlaceOrder()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var Order = _inventoryShoppingService.LoadAllInventoryOrders(userId);
                var maxOrderId = _inventoryOrderHistoryService.GetAll().Max(u => (long?)u.OrderId);
                long orderId = maxOrderId.HasValue ? maxOrderId.Value + 1 : 1;
                var garmentsProduct = _garmentsService.GetAllP();
                if (Order.Any())
                {
                    foreach (var order in Order)
                    {
                        var gProduct = _garmentsService.GetGarmentsProductById(order.GarmentsProduct.Id);
                        var rank = Convert.ToInt32(_inventoryOrderHistoryService.GetAll().Max(u => u.Rank));
                        OrderHistory orderHistory = new OrderHistory
                        {
                            GarmentsProduct = gProduct,
                            EmployeeId = order.EmployeeId,
                            Quantity = order.Count,
                            Price = gProduct.Price,
                            GarmentsId = order.GarmentsId,
                            OrderId = orderId,
                            CreatedBy = userId,
                            CreationDate = DateTime.Now,
                            Rank = rank + 1,
                            VersionNumber = 1,
                            Status = 1,
                            BusinessId = Guid.NewGuid().ToString()
                        };
                        orderHistory.TotalPrice = CalculateTotalPrice(order.EmployeeId);
                        _inventoryOrderHistoryService.Add(orderHistory);

                        var productRank = _product.GetAllProduct().Count();
                        Product product = new Product
                        {
                            Name = gProduct.Name,
                            Image = gProduct.Image,
                            SKU = gProduct.SKU,
                            Quantity = order.Count,
                            ProductType = gProduct.ProductType,
                            Department = gProduct.Department,
                            Description = gProduct.Description,
                            BuyingPrice = gProduct.Price,
                            BusinessId = Guid.NewGuid().ToString(),
                            CreatedBy = userId,
                            CreationDate = DateTime.Now,
                            Status = 0,
                            Rank = productRank + 1,
                            OrderHistoryId = orderHistory.Id,
                            ProductCode = (int)gProduct.ProductCode,
                            GarmentsId = gProduct.GarmentsId
                        };
                        _product.Add(product);

                    }
                    foreach (var order in Order)
                    {
                        _inventoryShoppingService.RemoveProductFromList(order);
                    }

                }
                Session.Remove("InventoryCartItemCount");
                return RedirectToAction("Invoice", "ManagerHome", new { area = "Manager", orderId = orderId });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

        }
        #endregion

        #region Product Order History
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult History()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                
                if (role != null)
                {
                    var orderHistory = _inventoryOrderHistoryService.GetAll().OrderByDescending(u => u.Rank);
                    var garmentsProduct = _garmentsService.GetAllP();
                    List<OrderHistoryViewModel> History = new List<OrderHistoryViewModel>();
                    foreach (var order in orderHistory)
                    {
                        OrderHistoryViewModel orderHistoryViewModel = new OrderHistoryViewModel
                        {
                            OrderId = order.OrderId,
                            Image = garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Image,
                            Name = garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Name,
                            Department = garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Department.Name,
                            ProductType = garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.ProductType.Name,
                            Price = order.Price,
                            Quantity = order.Quantity,
                            TotalPrice = order.TotalPrice,
                            OrderDate = (DateTime)order.CreationDate,
                            EmployeeId = order.EmployeeId,
                            ProductId = order.GarmentsProduct.Id,
                            Rank = (int)order.Rank
                            //GarmentsName= 
                        };
                        History.Add(orderHistoryViewModel);
                    }
                    return View(History);
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

        #region History Details
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult HistoryDetails(long id, long orderId)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                
                if (role != null)
                {
                    var orderHistory = _inventoryOrderHistoryService.GetAll().OrderByDescending(u => u.Rank);
                    var prod = orderHistory.FirstOrDefault(u => u.GarmentsProduct.Id == id && u.OrderId == orderId);
                    var garmentsProduct = _garmentsService.GetGarmentsProductById(id);
                    OrderHistoryViewModel orderHistoryViewModel = new OrderHistoryViewModel
                    {
                        OrderId = orderId,
                        Image = garmentsProduct.Image,
                        Name = garmentsProduct.Name,
                        Department = garmentsProduct.Department.Name,
                        ProductType = garmentsProduct.ProductType.Name,
                        Price = prod.Price,
                        Quantity = prod.Quantity,
                        TotalPrice = prod.TotalPrice,
                        OrderDate = (DateTime)prod.CreationDate,
                        EmployeeId = prod.EmployeeId,
                        ProductId = id,
                        Description = garmentsProduct.Description,
                        SKU = garmentsProduct.SKU,

                    };
                    return View(orderHistoryViewModel);
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

        #region Calculate Total Price
        private decimal CalculateTotalPrice(long EmployeeId)
        {
            try
            {
                var orderCarts = _inventoryShoppingService.LoadAllInventoryOrders(EmployeeId);
                decimal total = orderCarts.Sum(cart => cart.GarmentsProduct.Price * cart.Count);
                return total;
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Staff Product Approval
        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult ApproveProduct()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId);
                
                if (role != null)
                {
                    var product = _product.LoadYetApprovedProduct();
                    Dictionary<long, string> garments = new Dictionary<long, string>();
                    Dictionary<long, long> orderId = new Dictionary<long, long>();

                    foreach (var item in product)
                    {
                        string garment = _supplier.GetSupplierByUserId(item.GarmentsId).Name;
                        garments.Add(item.Id, garment);

                        long oId = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).OrderId;
                        orderId.Add(item.OrderHistoryId, oId);
                    }

                    ApprovedProductViewModel approvedProductViewModel = new ApprovedProductViewModel
                    {
                        Products = product,
                        Gname = garments,
                        OrderIds = orderId
                    };
                    return View(approvedProductViewModel);
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
        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult EditByStaff(long id)
        {

            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId);
               
                if (role != null)
                {
                    var prod = _product.GetProductById(id);

                    if (prod == null)
                    {
                        throw new Exception($"Product with ID {id} not found.");
                    }

                    return View(prod);
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
        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult EditByStaff(long id, Product product)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId);
               
                if (role != null)
                {
                    var prod = _product.GetProductById(id);
                    if (prod != null)
                    {
                        prod.Approved = true;
                        prod.ApprovedBy = Convert.ToInt64(User.Identity.GetUserId());
                        prod.ModificationDate = DateTime.Now;
                        prod.VersionNumber = 1;
                        _product.UpdateProduct(prod);
                        TempData["success"] = "Product Approved Successfully!";
                    }
                    return RedirectToAction("ApproveProduct");
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

        #region Reject the product by staff
        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult RejectByStaff(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId);
                
                if (role != null)
                {
                    var prod = _product.GetProductById(id);
                    if (prod != null)
                    {
                        prod.Approved = false;
                        prod.Rejected = true;
                        prod.ApprovedBy = Convert.ToInt64(User.Identity.GetUserId());
                        prod.ModificationDate = DateTime.Now;
                        prod.VersionNumber = 1;
                        _product.UpdateProduct(prod);
                        TempData["success"] = "Product has been Rejected!";
                    }
                    return RedirectToAction("ApproveProduct");
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

        #region Rejected Product List
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult RejectedProductList()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorizeRole(userId);

                if (role == "Admin" || role == "Staff")
                {
                    var prod = _product.LoadAllRejectedProducts();
                    Dictionary<long, long> orderId = new Dictionary<long, long>();
                    Dictionary<long, string> staffs = new Dictionary<long, string>();
                    Dictionary<long, string> garments = new Dictionary<long, string>();

                    foreach (var item in prod)
                    {
                        long oId = _inventoryOrderHistoryService.GetById(item.OrderHistoryId).OrderId;
                        orderId.Add(item.OrderHistoryId, oId);

                        staffs.Add(item.OrderHistoryId, GetUserEmailById(item.ApprovedBy));

                        string sup = _supplier.GetSupplierByUserId(item.GarmentsId).Name;
                        garments.Add(item.OrderHistoryId, sup);
                    }

                    StaffDashboardViewModel staffDashboardViewModel = new StaffDashboardViewModel
                    {
                        Products = prod,
                        OrderIds = orderId,
                        GName = garments,
                        StaffName = staffs
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

        #region Manage Price
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult ManagePrice()
        {
            
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var product = _product.GetAllNewProduct();
                    Dictionary<long, string> garments = new Dictionary<long, string>();
                    foreach (var item in product)
                    {
                        string garment = _supplier.GetSupplierByUserId(item.GarmentsId).Name;
                        garments.Add(item.Id, garment);
                    }
                    ApprovedProductViewModel approvedProductViewModel = new ApprovedProductViewModel
                    {
                        Products = product,
                        Gname = garments
                    };
                    return View(approvedProductViewModel);
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

        #region Set Price and Status by Manager
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult SetPrice(long id)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var prod = _product.GetProductById(id);

                    if (prod != null)
                    {
                        return View(prod);
                    }
                    return RedirectToAction("ManagePrice");
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
        [Authorize(Roles = "Manager,Admin")]
        [ValidateInput(false)]
        public ActionResult SetPrice(long id, Product product, HttpPostedFileBase ImageFile)
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                string role = IsAuthorize(userId);
                if (role != null)
                {
                    var prod = _product.GetProductById(id);

                    if (prod != null)
                    {
                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {

                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                            var path = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                            ImageFile.SaveAs(path);

                            prod.Image = uniqueFileName;
                        }

                        var existingProduct = _product.GetProductByProductCode(prod.ProductCode);

                        if (existingProduct != null)
                        {
                            existingProduct.Quantity = prod.Quantity + existingProduct.Quantity;
                            existingProduct.Price = product.Price;
                            existingProduct.Name = product.Name;
                            existingProduct.Description = product.Description;

                            if (!string.IsNullOrEmpty(prod.Image))
                            {
                                existingProduct.Image = prod.Image;
                            }
                            else
                            {
                                existingProduct.Image = existingProduct.Image;
                            }
                            existingProduct.IsPriceAdded = true;
                            existingProduct.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());//ManagerId
                            existingProduct.Status = 1;
                            existingProduct.VersionNumber = existingProduct.VersionNumber + 1;
                            _product.UpdateProduct(existingProduct);
                            TempData["success"] = "Product Added to Inventory Successfully!";
                            _product.DeleteProduct(prod);
                            return RedirectToAction("ManagePrice");
                        }
                        else
                        {

                            prod.Price = product.Price;
                            prod.Name = product.Name;
                            prod.Image = prod.Image;
                            prod.Description = product.Description;
                            prod.IsPriceAdded = true;
                            prod.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());//ManagerId
                            prod.Status = 1;
                            prod.VersionNumber = prod.VersionNumber + 1;

                            _product.UpdateProduct(prod);
                            TempData["success"] = "Product Added to Inventory Successfully!";
                            return RedirectToAction("ManagePrice");
                        }

                    }

                    TempData["error"] = "Product is not added to inventory!";
                    return RedirectToAction("ManagePrice");
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