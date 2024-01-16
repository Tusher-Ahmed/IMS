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
using System.Web.UI.WebControls;

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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<Department> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value;}
        }
        public DepartmentService()
        {
            _repository = new BaseDAO<Department>();
        }
        #region Add Department
        public void AddDept(Department dept)
        {           
            if (dept == null)
            {
                throw new ArgumentNullException(nameof(dept));
            }

            int highRank;
            try
            {
                highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

            using (var transaction = _session.BeginTransaction())
            {
                try
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

                    _repository.Add(department);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
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
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }
                if (data != null)
                {
                    try
                    {
                        _repository.Delete(data);
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
        #endregion

        #region Get All Department
        public IEnumerable<Department> GetAllDept()
        {
            try
            {
                return _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
           
        }
        #endregion

        #region Get Department By Id
        public Department GetDeptById(long id)
        {
            try
            {
                return _repository.GetById(id);

            }catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region Update Deparment
        public void UpdateDept(long id,Department dept)
        {
            using (var transaction = _session.BeginTransaction())
            {
                
                try
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
                    _repository.Update(deptData);
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
        #endregion
    }
}
