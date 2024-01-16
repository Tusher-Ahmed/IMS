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
    public interface ISupplierService
    {
        void AddSupplier(Supplier supplier);
        Supplier GetSupplierById(long id);
        IEnumerable<Supplier> GetSuppliers();
        Supplier GetSupplierByUserId(long userId);
    }
    public class SupplierService:ISupplierService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<Supplier> _repository;
        private readonly ISupplierDao _supplierDao;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _supplierDao.Session = value; }
        }
        public SupplierService()
        {
            _repository = new BaseDAO<Supplier>();
            _supplierDao = new SupplierDao();
        }

        public void AddSupplier(Supplier supplier)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Add(supplier);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Supplier GetSupplierById(long id)
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

        public IEnumerable<Supplier> GetSuppliers()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public Supplier GetSupplierByUserId(long userId)
        {
            try
            {
               return _supplierDao.GetSupplierByUserId(userId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
           
        }
    }
}
