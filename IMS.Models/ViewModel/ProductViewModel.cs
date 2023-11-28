using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class ProductViewModel
    {
     
        public List<Product> Products { get; set; }
        public IEnumerable<GarmentsProduct> GarmentsProducts { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public List<Department> Departments { get; set; }

        public string SearchProductName { get; set; }
        public int? SearchProductTypeId { get; set; }
        public int? SearchDepartmentId { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
