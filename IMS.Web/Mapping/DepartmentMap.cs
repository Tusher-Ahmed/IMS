using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Mapping
{
    public class DepartmentMap: ClassMap<Department>
    {
        public DepartmentMap()
        {
            Id(x => x.DepartmentId);
            Map(x => x.DepartmentName);
            Map(x => x.CreatedBy);
            Map(x => x.CreationDate);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.Status);
            Map(x => x.Rank);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);
            Table("Department");


        }
    }
}