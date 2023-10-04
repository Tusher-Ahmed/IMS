using IMS.DataAccess;
using IMS.Models;
using IMS.Service;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: Department
        private readonly DepartmentService _department;
        public DepartmentController(ISession session)
        {
            _department = new DepartmentService();
            _department.Session= session;
            
        }
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
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Department dept)
        {
            TempData["data"] = "";


            if (ModelState.IsValid)
            {               
                try
                {
                    _department.AddDept(dept);
                    TempData["data"] = "Department Created Successfully.";
                    return Json(new { message = TempData["data"] });

                }
                catch
                {
                    TempData["data"] = "Data is not inserted!!"; ;
                    return Json(new { message = TempData["data"] });

                }

            }
            return View();
        }

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
            if (ModelState.IsValid)
            {  
                try
                {
                    var deptData = _department.GetDeptById(id);
                    if(deptData.Status!=null) { dept.Status = deptData.Status; }
                    dept.ModifyBy = 3;
                    _department.UpdateDept(id,dept);
                    TempData["Edata"] = "Department Updated Successfully.";
                    return Json(TempData["Edata"]);
                }
                catch
                {
                    TempData["Edata"] = "Data is not Updated!!";
                    return Json(TempData["Edata"]);
                }
            }
            return View(dept);

        }

        public ActionResult Delete(long id)
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
        public ActionResult Delete(long id, Department dept)
        {
            if (dept != null)
            {
                _department.DeleteDept(id);
                return RedirectToAction("Index");
            }
            return View();
        }

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
                deptarment.ModifyBy = 3;               
                _department.UpdateDept(id, deptarment);
                return RedirectToAction("Index", "Department");
            }
            return View();
        }
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
                return PartialView("_SearchDeactivateDepartment", searchItem);
            }
            else
            {

                var data = _department.GetAllDept().Where(u=>u.Status==0);
                return PartialView("_SearchDeactivateDepartment", data);
            }
        }

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
                deptarment.ModifyBy = 4;
                _department.UpdateDept(id, deptarment);
                return RedirectToAction("Deactivate", "Department");
            }
            return View();
        }
    }
}