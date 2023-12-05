using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class GarmentsOrderHistoryViewModel
    {
        public IEnumerable<OrderHistory> OrderHistory { get; set; }
        public Dictionary<long, decimal> TotalPrice { get; set; }
        public Dictionary<long, string> OrderBy { get; set; }
    }
}
