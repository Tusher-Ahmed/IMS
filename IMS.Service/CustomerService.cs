using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface ICustomerService
    {
        void AddCustomer(Customer customer);
        Customer GetCustomerByUserId(long userId);
    }

    public class CustomerService:ICustomerService
    {
        private readonly BaseDAO<Customer> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public CustomerService()
        {
            _repository = new BaseDAO<Customer>();
        }

        public void AddCustomer(Customer customer)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Add(customer);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Customer GetCustomerByUserId(long userId)
        {
            return _session.Query<Customer>().FirstOrDefault(u=>u.UserId == userId);
        }
    }
}
