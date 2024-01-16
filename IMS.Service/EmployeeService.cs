using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IEmployeeService
    {
        void AddEmployee(Employee employee);
        Employee GetEmployeeById(long id);
        List<Employee> GetAllEmployee();
        Employee GetEmployeeByUserId(long userId);
        void UpdateEmployee(Employee employee);
    }
    public class EmployeeService:IEmployeeService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<Employee> _repository;
        private ISession _session;
        private readonly IEmployeeDao _employeeDao;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _employeeDao.Session = value; }
        }
        public EmployeeService()
        {
            _repository = new BaseDAO<Employee>();
            _employeeDao = new EmployeeDao();
        }
        public void AddEmployee(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Add(employee);
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
        public Employee GetEmployeeById(long id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }

        public List<Employee> GetAllEmployee()
        {
            try
            {
                //return _repository.GetAll();
                return _employeeDao.GetAllEmployee();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public Employee GetEmployeeByUserId(long userId)
        {
            try
            {
                return _employeeDao.GetEmployeeByUserId(userId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(employee);
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
    }
}
