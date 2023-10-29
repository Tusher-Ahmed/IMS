using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class OrderDetailMap:ClassMap<OrderDetail>
    {
        public OrderDetailMap()
        {
            Table("OrderDetail"); 

            Id(x => x.Id);
            Map(x => x.Count);
            Map(x => x.Price);
            Map(x => x.Rank);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.BusinessId);

            References(x => x.OrderHeader, "OrderHeaderId").Cascade.None();
            References(x => x.Product, "ProductId").Cascade.None();
        }
    }
}
