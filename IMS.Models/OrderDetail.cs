using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class OrderDetail
    {
        public virtual long Id { get; set; }
        public virtual long OrderHeaderId { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
        public virtual long ProductId { get; set;}
        public virtual Product Product { get; set; }
        public virtual int Count {  get; set; }
        public virtual decimal Price {  get; set; }
        public virtual int Rank { get; set;}
        public virtual long ModifyBy { get; set;}
        public virtual DateTime ModificationDate { get; set;}
        public virtual string BusinessId { get; set; }
    }
}
