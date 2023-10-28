using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class SupplierMap:ClassMap<Supplier>
    {
        public SupplierMap()
        {
            Table("Supplier");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.City);
            Map(x => x.StreetAddress);
            Map(x => x.Thana);
            Map(x => x.PostalCode);
            Map(x => x.UserId);
        }
    }
}
