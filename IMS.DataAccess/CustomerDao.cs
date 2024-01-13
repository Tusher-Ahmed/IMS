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
    public interface ICustomerDao
    {
        ISession Session { get; set; }
        Customer LoadCustomerById(long userId);
    }
    public class CustomerDao:BaseDAO<Customer>, ICustomerDao
    {
        public new ISession Session { get; set; }

        public Customer LoadCustomerById(long userId)
        {
            return Session.Query<Customer>().FirstOrDefault(u => u.UserId == userId);
        }
    }
    
}
