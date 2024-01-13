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
    public interface ICancelReasonService
    {
        void AddReason(CancelReason reason);
        CancelReason GetReasonByOrderHeaderId(long id);
    }
    public class CancelReasonService:ICancelReasonService
    {
        private readonly BaseDAO<CancelReason> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public CancelReasonService()
        {
            _repository = new BaseDAO<CancelReason>();
        }

        public void AddReason(CancelReason reason)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Add(reason);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        //TODO: Task: move it to repository
        public CancelReason GetReasonByOrderHeaderId(long id)
        {
            return Session.Query<CancelReason>().Where(u=>u.OrderHeader.Id == id).FirstOrDefault();
        }
    }
}
