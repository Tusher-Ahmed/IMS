using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class RejectedProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public Dictionary<long, string> Managers { get; set; }
        public Dictionary<long, string> Garments { get; set; }
        public Dictionary<long, string> Staffs { get; set; }
    }
}
