using FluentNHibernate.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ISession _session;
        private readonly ITransaction _transaction;

        public Repository(ISession session)
        {
            _session = session;
            _transaction = _session.BeginTransaction();
        }
        public void Add(T entity)
        {
            _session.Save(entity);
        }

        public void Delete(T entity)
        {
            _session.Delete(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _session.Query<T>().ToList();
        }

        public T GetById(long id)
        {
            return _session.Get<T>(id);
        }

        public void Update(T entity)
        {
            _session.Update(entity);
        }
        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
