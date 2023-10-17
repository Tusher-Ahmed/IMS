using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _product;
        private readonly IDepartmentService _department;
        private readonly IProductTypeService _productType;
        private readonly IInventoryShoppingService _inventoryShoppingService;
        private readonly IInventoryOrderHistoryService _inventoryOrderHistoryService;
        private readonly IGarmentsService _garmentsService;
        public ProductController(ISession session)
        {
            _product=new ProductService { Session=session};
            _department=new DepartmentService {  Session=session};
            _productType = new ProductTypeService { Session = session };
            _inventoryShoppingService = new InventoryShoppingService {  Session=session};
            _inventoryOrderHistoryService = new InventoryOrderHistoryService {  Session=session};
            _garmentsService=new GarmentsService { Session=session };
            
        }

        #region Get All Product
        // GET: Product
        public ActionResult Index(ProductViewModel product)
        {
            product = _product.GetProducts(product);
            product.ProductTypes = _productType.GetAllType().ToList();
            product.Departments=_department.GetAllDept().ToList();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ProductListPartial", product);
            }
            return View(product);
        }
        #endregion

        #region Product Details
        public ActionResult ProductDetails(long ProductId)
        {
            var product=_product.GetProductById(ProductId);
            if (product != null)
            {
                return View(product);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Inventory Place Order and Add product
        public ActionResult PlaceOrder()
        {
            var Order = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == 1);
            var orderId=_inventoryOrderHistoryService.GetAll().Count();
            var garmentsProduct = _garmentsService.GetAllP();
            if (Order.Any())
            {
                foreach(var order in Order)
                {
                    var gProduct= _garmentsService.GetGarmentsProductById(order.GarmentsProduct.Id);
                    var rank = Convert.ToInt32( _inventoryOrderHistoryService.GetAll().Max(u => u.Rank));
                    OrderHistory orderHistory = new OrderHistory
                    {
                        GarmentsProduct = gProduct,
                        EmployeeId = order.EmployeeId,
                        Quantity = order.Count,
                        Price = gProduct.Price,
                        GarmentsId = order.GarmentsId,
                        OrderId = orderId + 1,
                        CreatedBy=order.EmployeeId,
                        CreationDate=DateTime.Now,
                        Rank=rank+1,
                        VersionNumber=1,
                        Status=1,
                        BusinessId= Guid.NewGuid().ToString()
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
                        CreatedBy = order.EmployeeId,
                        CreationDate = DateTime.Now,
                        Status = 0,
                        Rank = productRank + 1,
                        OrderHistoryId = orderHistory.Id,
                        ProductCode= (int)gProduct.ProductCode,
                        GarmentsId=gProduct.GarmentsId
                    };
                    _product.Add(product);

                }
                foreach(var order in Order)
                {
                    _inventoryShoppingService.RemoveProductFromList(order);
                }

            }
            
            return RedirectToAction("Index","Garments");
        }
        #endregion

        #region Product Order History
        public ActionResult History()
        {
            var orderHistory = _inventoryOrderHistoryService.GetAll().OrderByDescending(u => u.Rank);
            var garmentsProduct = _garmentsService.GetAllP();
            List<OrderHistoryViewModel> History=new List<OrderHistoryViewModel>();
            foreach(var order in orderHistory)
            {
                OrderHistoryViewModel orderHistoryViewModel = new OrderHistoryViewModel
                {
                    OrderId = order.OrderId,
                    Image = garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Image,
                    Name= garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Name,
                    Department= garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.Department.Name,
                    ProductType= garmentsProduct.FirstOrDefault(product => product.Id == order.GarmentsProduct.Id)?.ProductType.Name,
                    Price= order.Price,
                    Quantity=order.Quantity,
                    TotalPrice=order.TotalPrice,
                    OrderDate= (DateTime)order.CreationDate,
                    EmployeeId= order.EmployeeId,
                    ProductId=order.GarmentsProduct.Id,
                    Rank= (int)order.Rank
                    //GarmentsName= 
                };
                History.Add(orderHistoryViewModel);
            }
            return View(History);
        }
        #endregion

        #region History Details
        public ActionResult HistoryDetails(long id,long orderId)
        {
            var orderHistory = _inventoryOrderHistoryService.GetAll().OrderByDescending(u => u.Rank);
            var prod=orderHistory.FirstOrDefault(u=>u.GarmentsProduct.Id==id && u.OrderId==orderId);
            var garmentsProduct=_garmentsService.GetGarmentsProductById(id);
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
        #endregion

        #region Calculate Total Price
        private decimal CalculateTotalPrice(long EmployeeId)
        {
            var orderCarts = _inventoryShoppingService.GetAllInventoryOrders().Where(u => u.EmployeeId == EmployeeId).ToList();
            decimal total = orderCarts.Sum(cart => cart.GarmentsProduct.Price * cart.Count);
            return total;
        }
        #endregion

        #region Staff Product Approval
        public ActionResult ApproveProduct()
        {
            var product=_product.GetAllProduct().Where(u=>u.Approved==null).ToList();
            return View(product);
        }

        public ActionResult EditByStaff(long id)
        {
            var prod=_product.GetProductById(id);
            return View(prod);
        }

        [HttpPost]
        public ActionResult EditByStaff(long id, Product product)
        {
            var prod = _product.GetProductById(id);
            if (prod != null)
            {
                prod.Approved = true;
                prod.ApprovedBy = 1;
                prod.ModificationDate = DateTime.Now;
                prod.VersionNumber = 1;
                _product.UpdateProduct(prod);
            }
            return RedirectToAction("ApproveProduct");
        }
        #endregion

        #region Update Price and Status by Manager
        public ActionResult ManagePrice()
        {
            var product=_product.GetAllProduct().Where(u=>u.Approved==true && u.IsPriceAdded==false && u.Status==0);
            return View(product);
        }

        public ActionResult SetPrice(long id)
        {
            var prod = _product.GetProductById(id);
            if(prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("ManagePrice");
        }

        [HttpPost]
        public ActionResult SetPrice(long id,Product product)
        {
            var prod = _product.GetProductById(id);
            
            if (prod != null)
            {
                var existingProduct = _product.GetProductByProductCode(prod.ProductCode);
                if(existingProduct != null)
                {
                    existingProduct.Quantity=prod.Quantity+existingProduct.Quantity;
                    existingProduct.Price = product.Price;
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.IsPriceAdded = true;
                    existingProduct.ModifyBy = 2;//ManagerId
                    existingProduct.Status = 1;
                    existingProduct.VersionNumber = existingProduct.VersionNumber + 1;
                    _product.UpdateProduct(existingProduct);
                    _product.DeleteProduct(prod);
                    return RedirectToAction("ManagePrice");
                }
                else
                {
                    prod.Price = product.Price;
                    prod.Name = product.Name;
                    prod.Description = product.Description;
                    prod.IsPriceAdded = true;
                    prod.ModifyBy = 2;//ManagerId
                    prod.Status = 1;
                    prod.VersionNumber = prod.VersionNumber + 1;

                    _product.UpdateProduct(prod);
                    return RedirectToAction("ManagePrice");
                }
                
            }
            return RedirectToAction("ManagePrice");
        }
        #endregion
    }
}