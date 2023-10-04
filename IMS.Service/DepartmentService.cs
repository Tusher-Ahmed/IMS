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
    public class DepartmentService
    {
        private readonly Repository<Department> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public DepartmentService()
        {
            _repository = new Repository<Department>();
        }

        public void AddDept(Department dept)
        {
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            using (var transaction = _session.BeginTransaction())
            {
                Department department = new Department
                {
                    DepartmentName = dept.DepartmentName,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    Status = 1,
                    VersionNumber = 1,
                    Rank = highRank + 1,
                    BusinessId = Guid.NewGuid().ToString()
                };
                try
                {
                    _repository.Add(department);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
           
        }

        public void DeleteDept(long id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var data = _repository.GetById(id);
                if (data != null)
                {
                    try
                    {
                        _repository.Delete(data);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

        public void UpdateDept(long id,Department dept)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var deptData = _repository.GetById(id);
                if (deptData != null)
                {
                    deptData.DepartmentName = dept.DepartmentName;
                    deptData.ModifyBy = dept.ModifyBy;
                    deptData.Status = dept.Status;
                    deptData.ModificationDate = DateTime.Now;
                    deptData.VersionNumber = deptData.VersionNumber + 1;
                }
                try
                {
                    _repository.Update(deptData);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            
        }
    }
}
