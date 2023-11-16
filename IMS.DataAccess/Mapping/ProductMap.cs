using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DAO.Mapping
{
    public class ProductMap:ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Product"); 

            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Image);
            Map(x => x.Price);
            Map(x => x.SKU);
            Map(x => x.Quantity);
            Map(x => x.Description);
            Map(x => x.BuyingPrice);
            Map(x => x.IsPriceAdded);
            Map(x => x.ProductCode);
            Map(x => x.GarmentsId);
            Map(x => x.OrderHistoryId);
            Map(x => x.CreatedBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Approved);
            Map(x => x.Rejected);
            Map(x => x.ApprovedBy);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);

            References(x => x.Department)
                .Column("DepartmentId");
            References(x => x.ProductType)
                .Column("ProductTypeId");
        }
    }
}
