using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class GarmentsDashboardViewModel
    {
        public IEnumerable<GarmentsProduct> Products { get; set; }
        public int TotalProduct {  get; set; }
        public int TotalHistory {  get; set; }
        public IEnumerable<OrderHistory> OrderHistory { get; set; }
        public Dictionary<long, decimal> TotalPrice { get; set; }
        public Dictionary<long, string> OrderBy { get; set; }
    }
}
