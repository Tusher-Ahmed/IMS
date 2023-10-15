using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class InventoryOrderCartMap : ClassMap<InventoryOrderCart>
    {
        public InventoryOrderCartMap()
        {
            Table("InventoryOrderCart");
            Id(x => x.Id);
            Map(x => x.Count);
            Map(x => x.EmployeeId);
            Map(x => x.GarmentsId);
            References(x => x.GarmentsProduct, "ProductId").Column("ProductId").Cascade.None();
        }
    }
}
