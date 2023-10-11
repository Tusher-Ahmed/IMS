using IMS.Models.ViewModel;
using IMS.Service;
using NHibernate;
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
        public ProductController(ISession session)
        {
            _product=new ProductService { Session=session};
            _department=new DepartmentService {  Session=session};
            _productType = new ProductTypeService { Session = session };
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

        #region Create product
        public ActionResult Create()
        {

            var departments = _department.GetAllDept();
            var productTypes=_productType.GetAllType();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");

            return View();
        }


        #endregion
    }
}