using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class BuyingReportViewModel
    {
        public List<OrderHistory> History { get; set; }
        public Dictionary<long,string> Name {  get; set; }
    }
}
