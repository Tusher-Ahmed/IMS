using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class CancelReason
    {
        public virtual long Id { get; set; } 
        public virtual string OrderStatus {  get; set; }
        public virtual string PaymentStatus { get; set; }

        [Required(ErrorMessage = "The Cancel Reason field is required.")]
        public virtual string Reason {  get; set; }
        public virtual long OrderHeaderId {  get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
    }
}
