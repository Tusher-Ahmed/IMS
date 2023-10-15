using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class OrderHistoryMap:ClassMap<OrderHistory>
    {
        public OrderHistoryMap()
        {
            Table("OrderHistory");
            Id(x => x.Id);
            Map(x => x.EmployeeId);
            Map(x => x.GarmentsId);
            Map(x => x.OrderId);
            Map(x => x.TotalPrice);
            Map(x => x.Price);
            Map(x => x.Quantity);
            Map(x => x.CreatedBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);

            References(x => x.GarmentsProduct)
                .Column("ProductId");
        }
    }
}
