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
    public interface ICancelReasonService
    {
        void AddReason(CancelReason reason);
        CancelReason GetReasonByOrderHeaderId(long id);
    }
    public class CancelReasonService:ICancelReasonService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<CancelReason> _repository;
        private ISession _session;
        private readonly ICancelReasonDao _cancelReasonDao;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value;_cancelReasonDao.Session = value; }
        }
        public CancelReasonService()
        {
            _repository = new BaseDAO<CancelReason>();
            _cancelReasonDao = new CancelReasonDao();
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
                catch(Exception ex) 
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }
        }

        //TODO: Task: move it to repository
        public CancelReason GetReasonByOrderHeaderId(long id)
        {
            try
            {
                return _cancelReasonDao.GetReasonByOrderHeaderId(id);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
    }
}
