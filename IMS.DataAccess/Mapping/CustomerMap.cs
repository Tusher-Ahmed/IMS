using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.DAO.Mapping
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Table("Customer");
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.City);
            Map(x => x.StreetAddress);
            Map(x => x.Thana);
            Map(x => x.PostalCode);
            Map(x => x.UserId);
            Map(x => x.Status);
        }
    }
}