using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Models.ViewModel
{
    public class GarmentsProductViewModel
    {
       
        public IEnumerable<GarmentsProduct> GarmentsProducts { get; set; }
        public GarmentsProduct GarmentsProduct { get; set; }

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
