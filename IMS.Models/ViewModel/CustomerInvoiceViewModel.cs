using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class CustomerInvoiceViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public CancelReason CancelReason { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public string Email {  get; set; }
        public List<Product> Products { get; set; }
    }
}
