using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class StaffDashboardViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public List<string> SuppliersName { get; set; }
        public int TotalNewProduct {  get; set; }
        public int TotalApprovedProduct { get; set; }
        public Supplier Supplier { get; set; }
    }
}
