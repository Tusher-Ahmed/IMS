using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class GarmentsProduct
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual long GarmentsId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]{3,}(?: [a-zA-Z]+)*$", ErrorMessage = "Invalid Keyword!!")]
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Product Price is required.")]
        public virtual decimal Price { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        public virtual string SKU { get; set; }

        [Required]
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
        [Display(Name = "Select Depertment")]
        public virtual long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        [Display(Name = "Select Product Type")]
        public virtual long? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
