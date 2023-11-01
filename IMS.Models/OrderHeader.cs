using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class OrderHeader
    {
        public virtual long Id { get; set; }
        public virtual long CustomerId { get; set; }
        public virtual DateTime? OrderDate { get; set; }
        public virtual DateTime? ShippingDate { get; set; }
        public virtual decimal OrderTotal { get; set; }
        public virtual string OrderStatus { get; set; }
        public virtual string PaymentStatus { get; set; }
        public virtual string TrackingNumber { get; set; }
        public virtual string Carrier { get; set; }
        public virtual DateTime? PaymentDate { get; set; }
        public virtual string SessionId { get; set; }
        public virtual string PaymentIntentId { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set;}
        public virtual int VersionNumber { get;set; }
        public virtual string BusinessId { get; set; }
        [Display(Name ="Shop Name")]
        public virtual string Name {  get; set; }
        [Display(Name = "Phone")]
        public virtual string PhoneNumber {  get; set; }
        public virtual string City { get; set; }
        [Display(Name = "Street Address")]
        public virtual string StreetAddress { get; set; }
        public virtual string Thana { get; set; }
        public virtual string PostalCode { get; set; }
    }
}
