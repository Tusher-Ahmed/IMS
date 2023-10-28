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
        public int TotalProducts { get; set; }
        public int NewArrival {  get; set; }
        public int PendingOrder {  get; set; }
        public int TotalStaff {  get; set; }
        
    }
}
