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
    public interface ISupplierService
    {
        void AddSupplier(Supplier supplier);
        Supplier GetSupplierById(long id);
        IEnumerable<Supplier> GetSuppliers();
        Supplier GetSupplierByUserId(long userId);
    }
    public class SupplierService:ISupplierService
    {
        private readonly BaseDAO<Supplier> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public SupplierService()
        {
            _repository = new BaseDAO<Supplier>();
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
            return _repository.GetById(id);
        }

        public IEnumerable<Supplier> GetSuppliers()
        {
            return _repository.GetAll();
        }

        public Supplier GetSupplierByUserId(long userId)
        {
            return _session.Query<Supplier>().FirstOrDefault(u=>u.UserId== userId);
        }
    }
}
