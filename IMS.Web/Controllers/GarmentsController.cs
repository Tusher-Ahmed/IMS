using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class GarmentsController : Controller
    {
        private readonly IGarmentsService _garmentsService;
        private readonly IDepartmentService _departmentService;
        private readonly IProductTypeService _productTypeService;
        public GarmentsController(ISession session)
        {
            _garmentsService = new GarmentsService { Session = session };
            _departmentService = new DepartmentService { Session = session };
            _productTypeService = new ProductTypeService { Session = session };
        }
        // GET: Garments

        #region Garments Product Page
        public ActionResult Index(int pageNumber = 1)
        {
            var viewModel = _garmentsService.GetAllProduct(pageNumber);
            return View(viewModel);
        }
        #endregion

        #region Create Product
        public ActionResult Create()
        {
            var departments = _departmentService.GetAllDept();
            var productTypes = _productTypeService.GetAllType();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "ImageFile")] GarmentsProduct model, HttpPostedFileBase ImageFile)
        {

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);


                    model.Image = fileName;
                }
                model.ProductType = _productTypeService.GetProductTypeById((long)model.ProductTypeId);
                model.Department = _departmentService.GetDeptById((long)model.DepartmentId);

                _garmentsService.CreateGarmentsProduct(model);

                return RedirectToAction("Index");
            }
            


                // If ModelState is not valid, re-populate the dropdowns
                var departments = _departmentService.GetAllDept();
                var productTypes = _productTypeService.GetAllType();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");

                return View(model);
        }
         #endregion

        #region Product List
        public ActionResult ProductList()
        {
             var product = _garmentsService.GetAllP();
             return View(product);
        }
        #endregion
        #region Product Details
        public ActionResult Details(long id)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            return View(product);
        }
        #endregion
        #region Edit Product
        public ActionResult Edit(long id)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            var departments = _departmentService.GetAllDept();
            var productTypes = _productTypeService.GetAllType();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(long id, GarmentsProduct garmentsProduct)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            if (ModelState.IsValid)
            {
                garmentsProduct.ProductType = _productTypeService.GetProductTypeById((long)garmentsProduct.ProductTypeId);
                garmentsProduct.Department = _departmentService.GetDeptById((long)garmentsProduct.DepartmentId);
                _garmentsService.UpdateGarmentsProduct(id, garmentsProduct);
                return RedirectToAction("ProductList", "Garments");
            }
            var departments = _departmentService.GetAllDept();
            var productTypes = _productTypeService.GetAllType();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
            garmentsProduct.Image = product.Image;
            return View(garmentsProduct);
        }
        #endregion
    }
}