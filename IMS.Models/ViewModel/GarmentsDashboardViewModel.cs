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
        public List<OrderHistory> History { get; set; }
        public int TotalProduct {  get; set; }
        public int TotalHistory {  get; set; }
    }
}
