﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class CustomerShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> shoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
