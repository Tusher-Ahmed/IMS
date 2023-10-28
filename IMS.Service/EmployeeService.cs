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
    public interface IEmployeeService
    {
        void AddEmployee(Employee employee);
        Employee GetEmployeeById(long id);
        IEnumerable<Employee> GetAllEmployee();
        Employee GetEmployeeByUserId(long userId);
    }
    public class EmployeeService:IEmployeeService
    {
        private readonly BaseDAO<Employee> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public EmployeeService()
        {
            _repository = new BaseDAO<Employee>();
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }
        public Employee GetEmployeeById(long id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _repository.GetAll();
        }

        public Employee GetEmployeeByUserId(long userId)
        {
            return _session.Query<Employee>().FirstOrDefault(x => x.UserId == userId);
        }
    }
}
