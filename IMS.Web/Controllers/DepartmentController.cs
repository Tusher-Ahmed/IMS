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
    public class DepartmentController : Controller
    {
        // GET: Department
        private readonly IDepartmentService _department;
        public DepartmentController(ISession session)
        {
            _department = new DepartmentService { Session=session};
           
            
        }

        #region Index
        public ActionResult Index()
        {
            var data=_department.GetAllDept().Where(u=>u.Status==1);
            return View(data);
        }
        [HttpPost]
        public ActionResult Index(string dept)
        {
            if (string.IsNullOrEmpty(dept) == false)
            {
                var data = _department.GetAllDept();
                var searchItem=data.Where(u=>u.Name.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status==1).ToList();
                return PartialView("_SearchDepartment",searchItem);
            }
            else
            {

                var data = _department.GetAllDept();
                return PartialView("_SearchDepartment",data);
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
                    dept.CreatedBy= Convert.ToInt64(User.Identity.GetUserId());
                    _department.AddDept(dept);
                    TempData["data"] = "Department Created Successfully.";
                    //return Json(new { message = TempData["data"] });
                    return Json(new { success = true, message = TempData["data"] });

                }
                catch
                {
                    TempData["data"] = "Data is not inserted!!"; ;
                    //return Json(new { message = TempData["data"] });
                    return Json(new { success = false, message = TempData["data"] });

                }

            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = errors, errors = errors });
        }
        #endregion

        #region Edit
        public ActionResult Edit(long id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            var dept=_department.GetDeptById(id);
            if (dept != null)
            {
                return View(dept);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(long id,Department dept)
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
                    if(deptData.Status!=null) { dept.Status = deptData.Status; }
                    dept.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                    _department.UpdateDept(id,dept);
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
        #endregion

        #region Deactivate Status
        public ActionResult Status(long id)
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

        [HttpPost]
        public ActionResult Status(long id,Department dept)
        {
            var deptarment = _department.GetDeptById(id);
            if (deptarment != null)
            {
                deptarment.Status = 0;
                deptarment.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());               
                _department.UpdateDept(id, deptarment);
                return RedirectToAction("Index", "Department");
            }
            return View();
        }
        #endregion

        #region All Deactivate Status
        public ActionResult Deactivate()
        {
            var data = _department.GetAllDept().Where(u => u.Status == 0);
            return View(data);
        }
        [HttpPost]
        public ActionResult Deactivate(string dept)
        {
            if (string.IsNullOrEmpty(dept) == false)
            {
                var data = _department.GetAllDept();
                var searchItem = data.Where(u => u.Name.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0 && u.Status == 0).ToList();
                return PartialView("_SearchDepartment", searchItem);
            }
            else
            {

                var data = _department.GetAllDept().Where(u=>u.Status==0);
                return PartialView("_SearchDepartment", data);
            }
        }
        #endregion

        #region Active Status
        public ActionResult Active(long id)
        {
            var dept=_department.GetDeptById(id);
            if (dept != null)
            {
                return View(dept);
            }
            return RedirectToAction("Deactivate", "Department");
        }

        [HttpPost]
        public ActionResult Active(long id,Department dept)
        {
            var deptarment = _department.GetDeptById(id);
            if (deptarment != null)
            {
                deptarment.Status = 1;
                deptarment.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                _department.UpdateDept(id, deptarment);
                return RedirectToAction("Deactivate", "Department");
            }
            return View();
        }
        #endregion
    }
}