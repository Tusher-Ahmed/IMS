using Autofac.Integration.Mvc;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS.DataAccess;
using IMS.Service;

namespace IMS.Web.App_Start
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => NHibernateHelper.OpenSession())
                                     .As<NHibernate.ISession>()
                                     .InstancePerRequest();

            // Register your controllers with Autofac
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register your services and repositories here
             builder.RegisterType<DepartmentService>().As<IDepartmentService>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            // Build the Autofac container
            var container = builder.Build();

            // Set the DependencyResolver to use Autofac
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}