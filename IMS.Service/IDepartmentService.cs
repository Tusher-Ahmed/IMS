using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAllDept();
        Department GetDeptById(long id);
        void AddDept(Department todo);
        void UpdateDept(Department todo);
        void DeleteDept(long id);
    }
}
