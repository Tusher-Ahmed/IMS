using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class InventoryCartViewModel
    {
        public IEnumerable<InventoryOrderCart> OrderCarts { get; set; }
        public virtual decimal TotalPrice { get; set; }
    }
}
