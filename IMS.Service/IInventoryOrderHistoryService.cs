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
    public interface IInventoryOrderHistoryService
    {
        void Add(OrderHistory orderHistory);
        IEnumerable<OrderHistory> GetAll();
        IEnumerable<OrderHistory> GetByOrderId(long orderId);
    }
    public class InventoryOrderHistoryService:IInventoryOrderHistoryService
    {
        private readonly BaseDAO<OrderHistory> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }

        public InventoryOrderHistoryService()
        {
            _repository = new BaseDAO<OrderHistory>();
        }

        public void Add(OrderHistory orderHistory)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Add(orderHistory);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<OrderHistory> GetAll()
        {
            return _repository.GetAll();
        }
        public IEnumerable<OrderHistory> GetByOrderId(long orderId)
        {
            return _session.Query<OrderHistory>().Where(u=>u.OrderId == orderId);
        }

    }
}
