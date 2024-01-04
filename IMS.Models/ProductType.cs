using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class ProductType
    {
        public virtual long Id { get; set; }
        [Display(Name = "Product Type")]
        [Required(ErrorMessage = "Product Type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product Type must be between 3 and 50 characters.")]
        //[RegularExpression(@"^[a-zA-Z]{3,}(?: [a-zA-Z]+)*$", ErrorMessage = "Invalid Keyword!!")]
        public virtual string Name { get; set; }

        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual long? ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
    }
}
