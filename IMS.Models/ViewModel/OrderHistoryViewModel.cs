using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class OrderHistoryViewModel
    {
        public long OrderId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string ProductType { get; set; }
        public long ProductId { get; set; }
        public int Rank { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public decimal EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string GarmentsName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }

    }
}
