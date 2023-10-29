using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class OrderHeader
    {
        public virtual long Id { get; set; }
        public virtual long CustomerId { get; set; }
        public virtual DateTime ShippingDate { get; set; }
        public virtual long OrderTotal { get; set; }
        public virtual string OrderStatus { get; set; }
        public virtual string PaymentStatus { get; set; }
        public virtual string TrackingNumber { get; set; }
        public virtual string Carrier { get; set; }
        public virtual DateTime PaymentDate { get; set; }
        public virtual DateTime PaymentDueDate { get; set; }
        public virtual string PaymentIntentId { get; set; }
        public virtual string SessionId { get; set; }
        public virtual long ModifyBy { get; set; }
        public virtual DateTime ModificationDate { get; set;}
        public virtual int VersionNumber { get;set; }
        public virtual string BusinessId { get; set; }
        public virtual string Name {  get; set; }
        public virtual string City { get; set; }
        public virtual string StreetAddress { get; set; }
        public virtual string Thana { get; set; }
        public virtual string PostalCode { get; set; }
    }
}
