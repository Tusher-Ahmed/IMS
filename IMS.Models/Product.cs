﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Models
{
    public class Product
    {
        [Key]
        public virtual long Id { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product Name must be between 3 and 100 characters.")]
        // [RegularExpression(@"^[a-zA-Z]{3,}(?: [a-zA-Z]+)*$", ErrorMessage = "Invalid Keyword!!")]
        //[RegularExpression(@"^[a-zA-Z'""\(\)_\-, ]*$", ErrorMessage = "Invalid Product Name!!")]
        public virtual string Name { get; set; }
        [Required(ErrorMessage = "Product Image is required.")]
        [Display(Name = "Product Image")]
        public virtual string Image { get; set; }
        [Required(ErrorMessage = "Product Price is required.")]
        //[Range(1, 1000000, ErrorMessage = "Price must be non-negative and less than or equal to the maximum allowed value.")]
        [RegularExpression(@"^(?!0*(\.0{1,2})?$)([1-9]\d{0,5}(\.\d{1,2})?|1000000)$", ErrorMessage = "Price must be between 1 and maximum allowed value and at most two decimal value.")]
        public virtual decimal Price { get; set; }
        [Required(ErrorMessage = "Stock Keeping Unit is required")]
        public virtual string SKU { get; set; }
        [Required(ErrorMessage ="Product Quantity is Required.")]
        [Display(Name="Product Quantity")]
        public virtual int Quantity { get; set; }
        [Required]
        [AllowHtml]
        public virtual string Description { get; set; }
        public virtual int ProductCode { get; set; }
        public virtual long OrderHistoryId { get; set; }
        [Display(Name = "Buying Price")]
        public virtual decimal BuyingPrice { get; set; }
        public virtual bool IsPriceAdded { get; set; }
        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual long? ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get;set; }
        public virtual bool? Approved { get; set; }
        public virtual bool? Rejected { get; set; }
        public virtual long? ApprovedBy { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }
        public virtual long GarmentsId {  get; set; }

        public virtual Department Department { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
