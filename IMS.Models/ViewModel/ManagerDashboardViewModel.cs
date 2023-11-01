using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class ManagerDashboardViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public int TotalProducts { get; set; }
        public int NewArrival {  get; set; }
        public int Processing {  get; set; }
        public int TotalCancel {  get; set; }
        public int TotalStaff {  get; set; }
        public int NewOrder { get; set; }
        
    }
}
