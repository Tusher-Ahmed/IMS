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
        private readonly IDepartmentService _department;
        public DepartmentController(IDepartmentService department)
        {
            _department = department;    
        }
        public ActionResult Index()
        {
            var data=_department.GetAllDept();
            return View(data);
        }
        [HttpPost]
        public ActionResult Index(string dept)
        {
            if (string.IsNullOrEmpty(dept) == false)
            {
                var data = _department.GetAllDept();
                var searchItem=data.Where(u=>u.DepartmentName.IndexOf(dept, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
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
            int highRank =Convert.ToInt32( _department.GetAllDept().Max(u => u.Rank));
            if (ModelState.IsValid)
            {
                Department department = new Department
                {
                    DepartmentName = dept.DepartmentName,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    Status = 1,
                    VersionNumber = 1,
                    Rank = highRank + 1,
                    BusinessId = Guid.NewGuid().ToString()
                };               
                try
                {
                    _department.AddDept(department);
                    TempData["data"] = "Department Created Successfully.";
                    return Json(TempData["data"]);
                }
                catch
                {
                    TempData["data"] = "Data is not inserted!!";
                    return Json(TempData["data"]);
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
            var deptData=_department.GetDeptById(id);
            if (deptData == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                deptData.DepartmentName = dept.DepartmentName;
                deptData.ModifyBy = 2;
                deptData.ModificationDate= DateTime.Now;
                deptData.VersionNumber = deptData.VersionNumber + 1;
                try
                {
                    _department.AddDept(deptData);
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
    }
}