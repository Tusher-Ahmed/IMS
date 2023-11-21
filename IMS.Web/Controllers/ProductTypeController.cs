using IMS.Models;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductTypeController : BaseController
    {
        // GET: ProductType
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProductTypeService _productType;
        public ProductTypeController(ISession session):base(session)
        {
            _productType = new ProductTypeService { Session=session};
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Index
        public ActionResult Index()
        {
            try
            {
                var data = _productType.GetAllType();
                return View(data);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        public ActionResult Index(string pType)
        {
            try
            {
                if (string.IsNullOrEmpty(pType) == false)
                {
                    var data = _productType.GetAllType();
                    var searchItem = data.Where(u => u.Name.IndexOf(pType, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status == 1).ToList();
                    return PartialView("_SearchProductType", searchItem);
                }
                else
                {

                    var data = _productType.GetAllType();
                    return PartialView("_SearchProductType", data);
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }

            
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Create(ProductType pType)
        {
            try
            {
                TempData["data"] = "";
                var data = _productType.GetAllType().Where(u => u.Name == pType.Name);
                if (data.Any())
                {
                    ModelState.AddModelError("Name", "Product Type is already in use.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        pType.CreatedBy = Convert.ToInt64(User.Identity.GetUserId());
                        _productType.AddProductType(pType);
                        TempData["data"] = "Product Type Added Successfully.";
                        return Json(new { success = true, message = TempData["data"] });

                    }
                    catch
                    {
                        TempData["data"] = "Data is not inserted!!"; ;
                        return Json(new { success = false, message = TempData["data"] });

                    }

                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = errors, errors = errors });
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
                if (id == 0)
                {
                    return HttpNotFound();
                }
                var productType = _productType.GetProductTypeById(id);
                if (productType != null)
                {
                    return View(productType);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }

        [HttpPost]
        public ActionResult Edit(long id,ProductType pType)
        {
            try
            {
                if (pType == null)
                {
                    return HttpNotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        var deptData = _productType.GetProductTypeById(id);
                        if (deptData.Status != null) { pType.Status = deptData.Status; }
                        pType.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                        _productType.UpdateProductType(id, pType);
                        TempData["data"] = "Product Type Updated Successfully.";
                        return Json(new { success = true, message = TempData["data"] });
                    }
                    catch
                    {
                        TempData["data"] = "Data is not Updated!!";
                        return Json(new { success = false, message = TempData["data"] });
                    }
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = errors, errors = errors });
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }           
        }
        #endregion

        #region Deactivate Status
        public ActionResult Status(long id)
        {
            try
            {
                if (id == 0)
                {
                    return HttpNotFound();
                }
                var productType = _productType.GetProductTypeById(id);
                if (productType != null)
                {
                    return View(productType);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

        [HttpPost]
        public ActionResult Status(long id, Department dept)
        {
            try
            {
                var productType = _productType.GetProductTypeById(id);
                if (productType != null)
                {
                    productType.Status = 0;
                    productType.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _productType.UpdateProductType(id, productType);
                    TempData["success"] = "Status Updated Successfully!";
                    return RedirectToAction("Index", "ProductType");
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

        #region All Deactivate Status
        public ActionResult Deactivate()
        {
            try
            {
                var data = _productType.GetAllType().Where(u => u.Status == 0);
                return View(data);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        public ActionResult Deactivate(string dept)
        {
            try
            {
                if (string.IsNullOrEmpty(dept) == false)
                {
                    var data = _productType.GetAllType();
                    var searchItem = data.Where(u => u.Name.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status == 0).ToList();
                    return PartialView("_SearchProductType", searchItem);
                }
                else
                {

                    var data = _productType.GetAllType().Where(u => u.Status == 0);
                    return PartialView("_SearchProductType", data);
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        #endregion

        #region Active Status
        public ActionResult Active(long id)
        {
            try
            {
                var dept = _productType.GetProductTypeById(id);
                if (dept != null)
                {
                    return View(dept);
                }
                return RedirectToAction("Deactivate", "ProductType");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

        [HttpPost]
        public ActionResult Active(long id, ProductType productType)
        {
            try
            {
                var ProductType = _productType.GetProductTypeById(id);
                if (ProductType != null)
                {
                    ProductType.Status = 1;
                    ProductType.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _productType.UpdateProductType(id, ProductType);
                    TempData["success"] = "Status Updated Successfully!";
                    return RedirectToAction("Deactivate", "ProductType");
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
    }
}