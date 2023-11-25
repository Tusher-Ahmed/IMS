using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class RejectedOrderViewModel
    {
       public List<Product> product { get; set; }
       public Dictionary<long, string> OrderBy { get; set; }

    }
}
