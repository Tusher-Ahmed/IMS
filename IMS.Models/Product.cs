﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class Product
    {
        [Key]
        public virtual long Id { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]{3,}(?: [a-zA-Z]+)*$", ErrorMessage = "Invalid Keyword!!")]
        public virtual string Name { get; set; }
        [Required(ErrorMessage = "Product Image is required.")]
        [Display(Name = "Product Image")]
        public virtual byte[] Image { get; set; }
        [Required(ErrorMessage = "Product Image is required.")]
        public virtual decimal Price { get; set; }
        [Required(ErrorMessage = "Stock Keeping Unit is required")]
        public virtual string SKU { get; set; }
        [Required(ErrorMessage ="Product Quantity is Required.")]
        [Display(Name="Product Quantity")]
        public virtual int Quantity { get; set; }
        [Required]
        public virtual string Description { get; set; }
        public virtual string ContentType { get; set; }
        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual long? ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get;set; }
        public virtual bool? Approved { get; set; }
        public virtual long? ApprovedBy { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}