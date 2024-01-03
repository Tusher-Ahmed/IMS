using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{

    public class GarmentsController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGarmentsService _garmentsService;
        private readonly IDepartmentService _departmentService;
        private readonly IProductTypeService _productTypeService;
        private readonly IManageProductService _manageProductService;
        public GarmentsController(ISession session) : base(session)
        {
            _garmentsService = new GarmentsService { Session = session };
            _departmentService = new DepartmentService { Session = session };
            _productTypeService = new ProductTypeService { Session = session };
            _manageProductService=new ManageProductService { Session = session };
            log4net.Config.XmlConfigurator.Configure();
        }
        // GET: Garments

        #region Garments Product Page
        [Authorize(Roles = "Admin,Manager,Supplier")]
        public ActionResult Index(GarmentsProductViewModel GarmentsProduct)
        {
            try
            {
                long supplierId = 0;
                if (User.IsInRole("Supplier"))
                {
                    supplierId = Convert.ToInt64(User.Identity.GetUserId());
                }
                GarmentsProduct = _garmentsService.GetAllProduct(GarmentsProduct, supplierId);
                GarmentsProduct.ProductTypes = _productTypeService.GetAllType().ToList();
                GarmentsProduct.Departments = _departmentService.GetAllDept().ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_GarmentsProductListPartial", GarmentsProduct);
                }

                return View(GarmentsProduct);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Create Product
        [Authorize(Roles = "Supplier")]
        public ActionResult Create()
        {
            try
            {
                var departments = _departmentService.GetAllDept();
                var productTypes = _productTypeService.GetAllType();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        [Authorize(Roles = "Supplier")]
        [ValidateInput(false)] // Allow HTML input
        public ActionResult Create([Bind(Exclude = "ImageFile")] GarmentsProduct model, HttpPostedFileBase ImageFile)
        {
            if (ImageFile == null || ImageFile.ContentLength <= 0)
            {
                ModelState.AddModelError("Image", "Image is required");
            }

            if(model.SKU == null)
            {
                ModelState.AddModelError("Description", "Please enter the description");
            }

            if (model.Description == null)
            {
                ModelState.AddModelError("SKU", "Please enter the description");
            }

            if (model.Name.Count(char.IsLetter) < 3)
            {
                ModelState.AddModelError("Name", "Product name must contain at least three letters.");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                    var path = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                    ImageFile.SaveAs(path);

                    model.Image = uniqueFileName;

                    // Set other product-related properties
                    model.ProductType = _productTypeService.GetProductTypeById((long)model.ProductTypeId);
                    model.Department = _departmentService.GetDeptById((long)model.DepartmentId);
                    model.GarmentsId = Convert.ToInt64(User.Identity.GetUserId());

                    // Save the product in your service
                    _garmentsService.CreateGarmentsProduct(model);
                    TempData["success"] = "Product Added Successfully";

                    return RedirectToAction("ProductList");
                }

                var dept = _departmentService.GetAllDept();
                var productT = _productTypeService.GetAllType();
                ViewBag.Departments = new SelectList(dept, "Id", "Name");
                ViewBag.ProductTypes = new SelectList(productT, "Id", "Name");

                return View(model);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region Product List
        [Authorize(Roles = "Supplier")]
        public ActionResult ProductList()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                //var product = _garmentsService.GetAllP().Where(u => u.Status == 1 && u.GarmentsId == userId);
                var product = _garmentsService.GetAllPro(1, userId);
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
        [Authorize(Roles = "Supplier")]
        public ActionResult Details(long id)
        {
            try
            {
                var product = _garmentsService.GetGarmentsProductById(id);
                return View(product);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Edit Product
        [Authorize(Roles = "Supplier")]
        public ActionResult Edit(long id)
        {
            try
            {
                var product = _garmentsService.GetGarmentsProductById(id);
                var departments = _departmentService.GetAllDept();
                var productTypes = _productTypeService.GetAllType();
                ViewBag.Departments = new SelectList(departments, "Id", "Name", product.Department.Id);
                ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name", product.ProductType.Id);

                var skus = string.IsNullOrEmpty(product.SKU)
                                        ? new List<string>()
                                        : product.SKU.Split(',').ToList();

                GarmentsEditViewModel garmentsEditView = new GarmentsEditViewModel
                {
                    GarmentsProduct = product,
                    SelectedSKUs = skus
                };

                return View(garmentsEditView);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        [Authorize(Roles = "Supplier")]
        public ActionResult Edit(long id, GarmentsEditViewModel gv, HttpPostedFileBase ImageFile)
        {
            try
            {
                var product = _garmentsService.GetGarmentsProductById(id);
                gv.GarmentsProduct.Image = product.Image;

                if (gv.GarmentsProduct.Name.Count(char.IsLetter) < 3)
                {
                    ModelState.AddModelError("GarmentsProduct.Name", "Product name must contain at least three letters.");
                }
                if (gv.GarmentsProduct.Description == null)
                {
                    ModelState.AddModelError("Description", "Description is required");                   
                }

                if (gv.SelectedSKUs != null)
                {
                    gv.GarmentsProduct.SKU = string.Join(",", gv.SelectedSKUs);
                }
                else
                {
                    ModelState.AddModelError("SelectedSKUs", "Product Sizes are required!!");
                }
                

                if (ModelState.IsValid)
                {
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                        var path = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                        ImageFile.SaveAs(path);

                        gv.GarmentsProduct.Image = uniqueFileName;
                    }                   

                    gv.GarmentsProduct.ProductType = _productTypeService.GetProductTypeById((long)gv.GarmentsProduct.ProductTypeId);
                    gv.GarmentsProduct.Department = _departmentService.GetDeptById((long)gv.GarmentsProduct.DepartmentId);
                    gv.GarmentsProduct.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _garmentsService.UpdateGarmentsProduct(id, gv.GarmentsProduct);
                    TempData["success"] = "Product Updated Successfully";

                    return RedirectToAction("ProductList", "Garments");
                }

                var departments = _departmentService.GetAllDept();
                var productTypes = _productTypeService.GetAllType();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
                

                return View(gv);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

            
        }
        #endregion

        #region Status & Deactivation
        [Authorize(Roles = "Supplier")]
        public ActionResult Status(long id)
        {
            try
            {
                var product = _garmentsService.GetGarmentsProductById(id);
                return View(product);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "Supplier")]
        public ActionResult Status(long id, GarmentsProduct garmentsProduct)
        {
            try
            {
                var product = _garmentsService.GetGarmentsProductById(id);
                if (product != null)
                {
                    product.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _garmentsService.UpdateStatus(id, product);
                    TempData["success"] = "Status Updated Successfully";
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

        #region Show deactivated Product List
        [Authorize(Roles = "Supplier")]
        public ActionResult DeactivatedList()
        {
            try
            {
                long userId = Convert.ToInt64(User.Identity.GetUserId());
                var prod = _garmentsService.GetAllP().Where(u => u.Status == 0 && u.GarmentsId == userId);
                return View(prod);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }
        #endregion

        #region Activate Product from Deactivation
        [Authorize(Roles = "Supplier")]
        public ActionResult ActiveStatus(long id)
        {
            try
            {
                var prod = _garmentsService.GetGarmentsProductById(id);
                if (prod != null)
                {
                    return View(prod);
                }
                return RedirectToAction("DeactivatedList");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        [Authorize(Roles = "Supplier")]
        public ActionResult ActiveStatus(long id, GarmentsProduct garmentsProduct)
        {
            try
            {
                var prod = _garmentsService.GetGarmentsProductById(id);
                if (prod != null)
                {
                    prod.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _garmentsService.ActivateStatus(id, prod);
                    TempData["success"] = "Status Updated Successfully";
                    return RedirectToAction("DeactivatedList", "Garments");
                }
                return View(prod);
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