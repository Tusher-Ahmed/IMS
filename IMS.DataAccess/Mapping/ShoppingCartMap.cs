using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class ShoppingCartMap:ClassMap<ShoppingCart>
    {
        public ShoppingCartMap()
        {
            Table("ShoppingCart");
            Id(x => x.Id);
            Map(x => x.Count);
            Map(x => x.CustomerId);
            References(x => x.Product, "ProductId").Column("ProductId").Cascade.None();
        }
    }
}
