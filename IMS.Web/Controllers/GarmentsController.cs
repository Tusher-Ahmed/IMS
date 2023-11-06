using IMS.Models;
using IMS.Models.ViewModel;
using IMS.Service;
using Microsoft.AspNet.Identity;
using NHibernate;
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
        [Authorize(Roles ="Admin,Manager,Supplier")]
        public ActionResult Index(int pageNumber = 1)
        {
            var viewModel = _garmentsService.GetAllProduct(pageNumber);
            return View(viewModel);
        }
        #endregion

        #region Create Product
        [Authorize(Roles = "Supplier")]
        public ActionResult Create()
        {
            var departments = _departmentService.GetAllDept();
            var productTypes = _productTypeService.GetAllType();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.ProductTypes = new SelectList(productTypes, "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Supplier")]
        [ValidateInput(false)] // Allow HTML input
        public ActionResult Create([Bind(Exclude = "ImageFile")] GarmentsProduct model)
        {
            if (ModelState.IsValid)
            {
                var (processedDescription, primaryImageUrl) = ProcessDescription(model.Description);

                // Set the model.Description to the processed description
                model.Description = processedDescription;
                if (string.IsNullOrEmpty(model.Description))
                {
                    ModelState.AddModelError("Description", "Product Description Is Required.");
                    var dept = _departmentService.GetAllDept();
                    var prodType = _productTypeService.GetAllType();
                    ViewBag.Departments = new SelectList(dept, "Id", "Name");
                    ViewBag.ProductTypes = new SelectList(prodType, "Id", "Name");
                    return View(model);
                }
                model.Image = primaryImageUrl;
                
                // Set the primary image URL
                
                if (string.IsNullOrEmpty(model.Image))
                {
                    ModelState.AddModelError("Image", "Image Is Required.");
                    var dept = _departmentService.GetAllDept();
                    var prodType = _productTypeService.GetAllType();
                    ViewBag.Departments = new SelectList(dept, "Id", "Name");
                    ViewBag.ProductTypes = new SelectList(prodType, "Id", "Name");
                    return View(model);
                }

                // Set other product-related properties
                model.ProductType = _productTypeService.GetProductTypeById((long)model.ProductTypeId);
                model.Department = _departmentService.GetDeptById((long)model.DepartmentId);
                model.GarmentsId = Convert.ToInt64(User.Identity.GetUserId());

                // Save the product in your service
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
        private (string, string) ProcessDescription(string description)
        {
           
            string pattern = "<img.*?src=[\"'](.*?)[\"'].*?>";
            var match = Regex.Match(description, pattern);

            if (match.Success)
            {
                string dataUri = match.Groups[1].Value;

                if (dataUri.StartsWith("data:image/"))
                {
                    byte[] imageBytes = Convert.FromBase64String(dataUri.Split(',')[1]);

                    // Check the image size here
                    if (imageBytes.Length > 5 * 1024 * 1024) 
                    {                       
                        ModelState.AddModelError("Image", "Image size cannot exceed 5 MB.");
                        return (description, null);
                    }
                    string imageUrl = SaveDataUriAsImage(dataUri);

                    // Remove the embedded image from the description
                    string processedDescription = description.Replace(match.Value, string.Empty);

                    // Remove extra line breaks and white spaces
                    processedDescription = processedDescription.Trim();

                    return (processedDescription, imageUrl);
                }
            }

            // No embedded image found
            return (description, null);
        }

        private string SaveDataUriAsImage(string dataUri)
        {
            // Extract the file extension from the data URI
            string extension = dataUri.Split(';')[0].Split('/')[1];

            // Create a unique file name
            string fileName = Guid.NewGuid() + "." + extension;

            // Get the base64-encoded image data
            string base64Data = dataUri.Split(',')[1];

            // Decode and save the image as a file
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            string imagePath = Path.Combine(Server.MapPath("~/Images"), fileName); // Change this path to where you want to save the image
            System.IO.File.WriteAllBytes(imagePath, imageBytes);

            // Return the URL to the saved image
            return  fileName; // Adjust the path as needed
        }
        #endregion

        #region Product List
        [Authorize(Roles = "Supplier")]
        public ActionResult ProductList()
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            var product = _garmentsService.GetAllP().Where(u => u.Status == 1 && u.GarmentsId==userId);
             return View(product);
        }
        #endregion

        #region Product Details
        [Authorize(Roles = "Supplier")]
        public ActionResult Details(long id)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            return View(product);
        }
        #endregion

        #region Edit Product
        [Authorize(Roles = "Supplier")]
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
        [Authorize(Roles = "Supplier")]
        public ActionResult Edit(long id, GarmentsProduct garmentsProduct)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            if (ModelState.IsValid)
            {
                garmentsProduct.ProductType = _productTypeService.GetProductTypeById((long)garmentsProduct.ProductTypeId);
                garmentsProduct.Department = _departmentService.GetDeptById((long)garmentsProduct.DepartmentId);
                garmentsProduct.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
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

        #region Status & Deactivation
        [Authorize(Roles = "Supplier")]
        public ActionResult Status(long id)
        {
            var product = _garmentsService.GetGarmentsProductById(id);
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Supplier")]
        public ActionResult Status(long id,GarmentsProduct garmentsProduct)
        {
            var product=_garmentsService.GetGarmentsProductById(id);
            if (product != null)
            {
                product.ModifyBy = Convert.ToInt64(User.Identity.GetUserId());
                _garmentsService.UpdateStatus(id,product);
                return RedirectToAction("ProductList");
            }
            return View(product);
        }
        #endregion

        #region Show deactivated Product List
        [Authorize(Roles = "Supplier")]
        public ActionResult DeactivatedList()
        {
            long userId = Convert.ToInt64(User.Identity.GetUserId());
            var prod = _garmentsService.GetAllP().Where(u => u.Status == 0 && u.GarmentsId == userId);
            return View(prod);
        }
        #endregion

        #region Activate Product from Deactivation
        [Authorize(Roles = "Supplier")]
        public ActionResult ActiveStatus(long id)
        {
            var prod = _garmentsService.GetGarmentsProductById(id);
            if (prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("DeactivatedList");
        }
        [HttpPost]
        [Authorize(Roles = "Supplier")]
        public ActionResult ActiveStatus(long id,GarmentsProduct garmentsProduct)
        {
            var prod = _garmentsService.GetGarmentsProductById(id);
            if (prod != null)
            {
                prod.ModifyBy= Convert.ToInt64(User.Identity.GetUserId());
                _garmentsService.ActivateStatus(id,prod);
                return RedirectToAction("DeactivatedList", "Garments");
            }
            return View(prod);
        }
        #endregion

       
    }
} 