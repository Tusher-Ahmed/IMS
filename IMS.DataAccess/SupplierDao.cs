using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface ISupplierDao
    {
        ISession Session { get; set; }
        Supplier GetSupplierByUserId(long userId);
    }
    public class SupplierDao:BaseDAO<Supplier>, ISupplierDao
    {
        public new ISession Session { get; set; }

        public Supplier GetSupplierByUserId(long userId)
        {
            return Session.Query<Supplier>().FirstOrDefault(u => u.UserId == userId);
        }
    }
}
