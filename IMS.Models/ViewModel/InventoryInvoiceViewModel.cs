using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class InventoryInvoiceViewModel
    {
        public IEnumerable<OrderHistory> orderHistories { get; set; }
        public List<string> GarmentsName { get; set; }
        public string ManagerEmail {  get; set; }

    }
}
