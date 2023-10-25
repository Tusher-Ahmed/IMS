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
   public interface IDepartmentService
    {
        void AddDept(Department dept);
        void DeleteDept(long id);
        void UpdateDept(long id, Department dept);
        IEnumerable<Department> GetAllDept();
        Department GetDeptById(long id);
    }
    public class DepartmentService : IDepartmentService
    {
        private readonly BaseDAO<Department> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public DepartmentService()
        {
            _repository = new BaseDAO<Department>();
        }
        #region Add Department
        public void AddDept(Department dept)
        {
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            using (var transaction = _session.BeginTransaction())
            {
                Department department = new Department
                {
                    Name = dept.Name,
                    CreatedBy = dept.CreatedBy,
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
        #endregion

        #region Delete Department
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
        #endregion

        #region Get All Department
        public IEnumerable<Department> GetAllDept()
        {
           return _repository.GetAll().ToList();
        }
        #endregion

        #region Get Department By Id
        public Department GetDeptById(long id)
        {
            return _repository.GetById(id);
        }
        #endregion

        #region Update Deparment
        public void UpdateDept(long id,Department dept)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var deptData = _repository.GetById(id);
                if (deptData != null)
                {
                    deptData.Name = dept.Name;
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
        #endregion
    }
}
