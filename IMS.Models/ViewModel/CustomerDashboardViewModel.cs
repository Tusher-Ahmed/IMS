using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class CustomerDashboardViewModel
    {
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Product> Products { get; set; }
        public int TotalOrders {  get; set; }
        public int TotalCanceledOrder {  get; set; }
        public int NewArrival {  get; set; }
    }
}
