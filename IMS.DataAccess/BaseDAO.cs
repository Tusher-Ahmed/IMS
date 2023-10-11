using FluentNHibernate.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DAO
{
    public class BaseDAO<T>  where T : class
    {
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; }
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
       
    }
}
