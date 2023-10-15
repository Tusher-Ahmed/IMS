using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class OrderHistory
    {
        [Key]
        public virtual long Id { get; set; }
        public virtual long EmployeeId { get; set; }
        public virtual long GarmentsId { get; set; }
        public virtual long OrderId { get; set; }
        public virtual decimal TotalPrice { get; set; }
        public virtual int Quantity { get; set;}
        public virtual decimal Price { get; set;}
        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; }
        public virtual long? ModifyBy { get; set; }
        public virtual DateTime? ModificationDate { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Rank { get; set; }
        public virtual int? VersionNumber { get; set; }
        public virtual string BusinessId { get; set; }

        public virtual GarmentsProduct GarmentsProduct { get; set; }



    }
}
