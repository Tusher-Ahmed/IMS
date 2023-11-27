using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class ApprovedProductViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<long,string> Gname { get; set; }
        public Dictionary<long,long> OrderIds { get; set; }
    }
}
