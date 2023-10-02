using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using IMS.Models;
using NHibernate.Tool.hbm2ddl;
using IMS.Web.Mapping;

namespace IMS.Web
{
    public class NHibernateHelper
    {
        public static NHibernate.ISession OpenSession()
        {
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                  .ConnectionString(@"Server=DESKTOP-44VLE5Q;Database=InventoryManagementSystem;Integrated Security=True;")
                              .ShowSql()
                )
               .Mappings(m =>
                          m.FluentMappings
                              .AddFromAssemblyOf<DepartmentMap>())
                .ExposeConfiguration(cfg => new SchemaExport(cfg)
                                                .Create(false, false))
                .BuildSessionFactory();
               
            return sessionFactory.OpenSession();
        }
    }
}