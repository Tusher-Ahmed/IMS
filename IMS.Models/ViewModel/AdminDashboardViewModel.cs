using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class AdminDashboardViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<OrderHistoryViewModel> History { get; set; }
        public IEnumerable<OrderHeader> orderHeaders { get; set; }
        public int TotalProduct {  get; set; }
        public int TotalEmployee {  get; set; }
        public int TotalOrders { get; set; }
        public int TotalShop {  get; set; }
        public int TotalNewOrders {  get; set; }
        public int TotalCancelOrder { get; set; }


    }
}
