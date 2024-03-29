﻿using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using IMS.Models;
using NHibernate.Tool.hbm2ddl;
using IMS.DAO.Mapping;
using IMS.DataAccess.Mapping;

namespace IMS.Service
{
    public class NHibernateHelper
    {
        public static NHibernate.ISession OpenSession()
        {
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                  .ConnectionString(@"Server=DESKTOP-L4HFC5H\SQLEXPRESS;Database=InventoryManagementSystem;Integrated Security=True;")
                              .ShowSql()
                )
               .Mappings(m =>
               {
                   m.FluentMappings
                                 .AddFromAssemblyOf<DepartmentMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<ProductTypeMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<ProductMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<GarmentsProductMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<InventoryOrderCartMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<OrderHistoryMap>();
                   m.FluentMappings
                                 .AddFromAssemblyOf<ShoppingCartMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<CustomerMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<SupplierMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<EmployeeMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<OrderHeaderMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<OrderDetailMap>();
                   m.FluentMappings
                                .AddFromAssemblyOf<CancelReasonMap>();

               })
                .ExposeConfiguration(cfg => new SchemaExport(cfg)
                                                .Create(false, false))
                .BuildSessionFactory();
               
            return sessionFactory.OpenSession();
        }
    }
}