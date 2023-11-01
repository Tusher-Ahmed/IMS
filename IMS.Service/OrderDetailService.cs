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
    public interface IOrderDetailService
    {
        void Add(OrderDetail orderDetail);
        OrderDetail GetOrderDetailById(int id);
        OrderDetail GetOrderDetailByOrderHeaderId(long id);
        IEnumerable<OrderDetail> getAllOrderDetails();
    }
    public class OrderDetailService:IOrderDetailService
    {
        private readonly BaseDAO<OrderDetail> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public OrderDetailService()
        {
            _repository = new BaseDAO<OrderDetail>();
        }

        public void Add(OrderDetail orderDetail)
        {
            int highRank = _repository.GetAll().Select(u => u.Rank).DefaultIfEmpty(0).Max();
            using (var transaction = _session.BeginTransaction())
            {
                orderDetail.Rank= highRank+1;         
                orderDetail.BusinessId = Guid.NewGuid().ToString();
                try
                {
                    _repository.Add(orderDetail);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<OrderDetail> getAllOrderDetails()
        {
            return _repository.GetAll();
        }

        public OrderDetail GetOrderDetailById(int id)
        {
           return _repository.GetById(id);
        }

        public OrderDetail GetOrderDetailByOrderHeaderId(long id)
        {
            return _session.Query<OrderDetail>().Where(u=>u.OrderHeader.Id==id).FirstOrDefault();
        }
    }
}
