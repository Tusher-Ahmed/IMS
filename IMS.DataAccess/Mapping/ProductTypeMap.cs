using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class ProductTypeMap: ClassMap<ProductType>
    {
        public ProductTypeMap()
        {
            Table("ProductType");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.CreatedBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);            
        }
    }
}
