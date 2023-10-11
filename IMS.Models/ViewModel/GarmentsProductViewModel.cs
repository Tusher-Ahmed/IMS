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
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
