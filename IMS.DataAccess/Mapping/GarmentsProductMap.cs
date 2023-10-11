using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DAO.Mapping
{
    public class GarmentsProductMap:ClassMap<GarmentsProduct>
    {
        public GarmentsProductMap()
        {
            Table("GarmentsProduct"); 

            Id(x => x.Id);
            Map(x => x.GarmentsId);
            Map(x => x.Name);
            Map(x => x.Image);
            Map(x => x.Price);
            Map(x => x.SKU);
            Map(x => x.ProductCode);
            Map(x => x.Description);
            Map(x => x.CreatedBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);
            //Map(x => x.ProductTypeId).Column("ProductTypeId");
            //Map(x => x.DepartmentId).Column("DepartmentId");

            References(x => x.Department, "DepartmentId").Cascade.None();
            References(x => x.ProductType, "ProductTypeId").Cascade.None();
        }
    }
}
