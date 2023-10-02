using IMS.DataAccess;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _repository;
        public DepartmentService(IRepository<Department> repository)
        {
            _repository = repository; 
        }
        public void AddDept(Department dept)
        {
            try
            {
                _repository.Add(dept);
                _repository.Commit();
            }
            catch
            {
                _repository.Rollback();
                throw; 
            }
        }

        public void DeleteDept(long id)
        {
            var data=_repository.GetById(id);
            if (data != null)
            {
                try
                {
                    _repository.Delete(data);
                    _repository.Commit();
                }
                catch
                {
                    _repository.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<Department> GetAllDept()
        {
           return _repository.GetAll().ToList();
        }

        public Department GetDeptById(long id)
        {
            return _repository.GetById(id);
        }

        public void UpdateDept(Department dept)
        { 
            try
            {
                _repository.Update(dept);
                _repository.Commit();
            }
            catch
            {
                _repository.Rollback();
                throw;
            }
        }
    }
}
