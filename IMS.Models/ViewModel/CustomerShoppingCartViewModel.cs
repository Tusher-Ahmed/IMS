using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Models.ViewModel
{
    [Authorize(Roles ="Manager")]
    public class CustomerShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> shoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }

        public string CustomerEmail { get; set; }
        
    }
}
 