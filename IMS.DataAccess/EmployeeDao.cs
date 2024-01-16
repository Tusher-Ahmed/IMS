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
    public interface IEmployeeDao
    {
        ISession Session { get; set; }
        Employee GetEmployeeByUserId(long userId);
        List<Employee> GetAllEmployee();
    }
    public class EmployeeDao: BaseDAO<Employee>, IEmployeeDao
    {
        public new ISession Session { get; set; }

        public List<Employee> GetAllEmployee()
        {
            string res = $@"
SELECT *
FROM Employee AS E
WHERE ( E.Status = '1')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Employee));
            var result = iquery.List<Employee>().ToList();

            return result;
        }

        public Employee GetEmployeeByUserId(long userId)
        {
            return Session.Query<Employee>().FirstOrDefault(x => x.UserId == userId);
        }
    }
}
