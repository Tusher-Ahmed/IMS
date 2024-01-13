using IMS.DAO;
using IMS.DataAccess;
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
        IEnumerable<Customer> GetAllCustomer();
        void UpdateCustomer(Customer customer);
        
    }

    public class CustomerService:ICustomerService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<Customer> _repository;
        private readonly ICustomerDao _customerDao;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _customerDao.Session = value; }
        }
        public CustomerService()
        {
            _repository = new BaseDAO<Customer>();
            _customerDao = new CustomerDao();
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
                catch(Exception ex)
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }
        }

        public Customer GetCustomerByUserId(long userId)
        {
            try
            {
                return _customerDao.LoadCustomerById(userId);

            }catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            try
            {
                return _repository.GetAll();
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(customer);
                    transaction.Commit();
                }
                catch(Exception ex ) 
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }
        }
    }
}
