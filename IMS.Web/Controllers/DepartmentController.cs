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
    public class DepartmentController : BaseController
    {
        // GET: Department
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDepartmentService _department;
        public DepartmentController(ISession session):base(session)
        {
            _department = new DepartmentService { Session=session};
            log4net.Config.XmlConfigurator.Configure();
        }

        #region Index
        public ActionResult Index()
        {
            try
            {
                var data = _department.GetAllDept().Where(u => u.Status == 1);
                return View(data);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
            
        }
        [HttpPost]
        public ActionResult Index(string dept)
        {
            try
            {
                if (string.IsNullOrEmpty(dept) == false)
                {
                    var data = _department.GetAllDept();
                    var searchItem = data.Where(u => u.Name.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status == 1).ToList();
                    return PartialView("_SearchDepartment", searchItem);
                }
                else
                {

                    var data = _department.GetAllDept();
                    return PartialView("_SearchDepartment", data);
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
        public ActionResult Create(Department dept)
        {
            try
            {
                TempData["data"] = "";
                var data = _department.GetAllDept().Where(u => u.Name == dept.Name);
                if (data.Any())
                {
                    ModelState.AddModelError("Name", "Department name is already in use.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        dept.CreatedBy = Convert.ToInt64(User.Identity.GetUserId());
                        _department.AddDept(dept);
                        TempData["data"] = "Department Created Successfully.";
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
                var dept = _department.GetDeptById(id);
                if (dept != null)
                {
                    return View(dept);
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
        public ActionResult Edit(long id,Department dept)
        {
            try
            {
                if (dept == null)
                {
                    return HttpNotFound();
                }
                var data = _department.GetAllDept().Where(u => u.Name == dept.Name);

                if (data.Any())
                {
                    ModelState.AddModelError("Name", "Department name is already in use.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var deptData = _department.GetDeptById(id);
                        if (deptData.Status != null) { dept.Status = deptData.Status; }
                        dept.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                        _department.UpdateDept(id, dept);
                        TempData["data"] = "Department Updated Successfully.";
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
                var dept = _department.GetDeptById(id);
                if (dept != null)
                {
                    return View(dept);
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
        public ActionResult Status(long id,Department dept)
        {
            try
            {
                var deptarment = _department.GetDeptById(id);
                if (deptarment != null)
                {
                    deptarment.Status = 0;
                    deptarment.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _department.UpdateDept(id, deptarment);
                    TempData["success"] = "Status Updated Successfully";
                    return RedirectToAction("Index", "Department");
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
                var data = _department.GetAllDept().Where(u => u.Status == 0);
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
                    var data = _department.GetAllDept();
                    var searchItem = data.Where(u => u.Name.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status == 0).ToList();
                    return PartialView("_SearchDepartment", searchItem);
                }
                else
                {

                    var data = _department.GetAllDept().Where(u => u.Status == 0);
                    return PartialView("_SearchDepartment", data);
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
                var dept = _department.GetDeptById(id);
                if (dept != null)
                {
                    return View(dept);
                }
                return RedirectToAction("Deactivate", "Department");
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                return RedirectToAction("Index", "Error");
            }
           
        }

        [HttpPost]
        public ActionResult Active(long id,Department dept)
        {
            try
            {
                var deptarment = _department.GetDeptById(id);
                if (deptarment != null)
                {
                    deptarment.Status = 1;
                    deptarment.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _department.UpdateDept(id, deptarment);
                    TempData["success"] = "Status Updated Successfully";
                    return RedirectToAction("Deactivate", "Department");
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