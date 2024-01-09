using IMS.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace IMS.Models
{
    public class GarmentsProduct
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual long GarmentsId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 100 characters.")]
       // [RegularExpression(@"^[a-zA-Z0-9\s\-_.,&'""\]+$", ErrorMessage = "Product name can only contain letters, numbers, spaces, and some special characters.")]
       
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
        [Display(Name="Image")]
        public virtual IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Product Price is required.")]
        //[Range(1, 1000000, ErrorMessage = "Price must be non-negative and less than or equal to the maximum allowed value.")]
        [RegularExpression(@"^(?!0*(\.0{1,2})?$)([1-9]\d{0,5}(\.\d{1,2})?|1000000)$", ErrorMessage = "Price must be between 1 and maximum allowed value and at most two decimal value.")]
        public virtual decimal Price { get; set; }

        [Required(ErrorMessage = "Product Sizes are required")]
        public virtual string SKU { get; set; }      

        [Required(ErrorMessage = "Product Description is required.")]
        [AllowHtml]
        public virtual string Description { get; set; }
        public virtual int? ProductCode { get; set; }
        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual long? ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
        [Display(Name = "Depertment")]
        [Required(ErrorMessage = "Department is required.")]
        public virtual long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        [Display(Name = "Product Type")]
        [Required(ErrorMessage = "ProductType is required.")]
        public virtual long? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
