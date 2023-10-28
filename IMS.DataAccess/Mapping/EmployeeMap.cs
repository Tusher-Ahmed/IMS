using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class EmployeeMap:ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employee");
            Id(x => x.Id);
            Map(x => x.City);
            Map(x => x.StreetAddress);
            Map(x => x.Thana);
            Map(x => x.PostalCode);
            Map(x => x.UserId);
            Map(x => x.CreatedBy);
        }
    }
}
