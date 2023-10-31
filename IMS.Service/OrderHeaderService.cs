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
    public interface IOrderHeaderService
    {
        OrderHeader GetOrderHeaderById(long id);
        IEnumerable<OrderHeader> GetAllOrderHeaders();
        void AddOrderHeader(OrderHeader orderHeader);
        void Update(OrderHeader orderHeader);
        void UpdateStatus(long id, string orderStatus, string PaymentStatus=null);
        void UpdateStripeSessionAndIntent(long id, string sessionId);
    }
    public class OrderHeaderService:IOrderHeaderService
    {
        private readonly BaseDAO<OrderHeader> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public OrderHeaderService()
        {
            _repository = new BaseDAO<OrderHeader>();
        }

        public void AddOrderHeader(OrderHeader orderHeader)
        {
            
            using (var transaction = _session.BeginTransaction())
            {
                orderHeader.BusinessId = Guid.NewGuid().ToString();
                orderHeader.VersionNumber = 1;
                try
                {
                    _repository.Add(orderHeader);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Update(OrderHeader orderHeader)
        {
            using (var transaction = _session.BeginTransaction())
            {                
                try
                {
                    _repository.Update(orderHeader);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateStatus(long id, string orderStatus, string PaymentStatus = null)
        {
            var orderFromDb=_repository.GetById(id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (PaymentStatus != null)
                {
                    orderFromDb.PaymentStatus= PaymentStatus;
                }   
            }
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(orderFromDb);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateStripeSessionAndIntent(long id, string sessionId)
        {
            var orderFromDb = _repository.GetById(id);
            if (orderFromDb != null)
            {
                orderFromDb.SessionId = sessionId;
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        _repository.Update(orderFromDb);
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

        public OrderHeader GetOrderHeaderById(long id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<OrderHeader> GetAllOrderHeaders()
        {
            return _repository.GetAll();
        }
    }
}
