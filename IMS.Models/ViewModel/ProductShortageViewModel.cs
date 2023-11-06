using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class ProductShortageViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public List<int> ShortageCounts {  get; set; }
        public List<long> ProductIds {  get; set; }
    }
}
