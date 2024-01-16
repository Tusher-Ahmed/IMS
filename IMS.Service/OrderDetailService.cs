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
    public interface IOrderDetailService
    {
        void Add(OrderDetail orderDetail);
        OrderDetail GetOrderDetailById(int id);
        OrderDetail GetOrderDetailByOrderHeaderId(long id);
        List<OrderDetail> LoadAllOrdersDetails(long orderHeaderId);
    }
    public class OrderDetailService:IOrderDetailService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<OrderDetail> _repository;
        private readonly IOrderDetailsDao _orderDetailsDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value;_orderDetailsDao.Session = value; }
        }
        public OrderDetailService()
        {
            _repository = new BaseDAO<OrderDetail>();
            _orderDetailsDao = new OrderDetailsDao();
        }

        public void Add(OrderDetail orderDetail)
        {
            
            using (var transaction = _session.BeginTransaction())
            {
                int highRank = _repository.GetAll().Select(u => u.Rank).DefaultIfEmpty(0).Max();
                orderDetail.Rank= highRank+1;         
                orderDetail.BusinessId = Guid.NewGuid().ToString();
                try
                {
                    _repository.Add(orderDetail);
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


        public List<OrderDetail> LoadAllOrdersDetails( long orderHeaderId)
        {
            try
            {
                return _orderDetailsDao.LoadAllOrdersDetails(orderHeaderId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        public OrderDetail GetOrderDetailById(int id)
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

        public OrderDetail GetOrderDetailByOrderHeaderId(long id)
        {
            try
            {
                return _orderDetailsDao.GetOrderDetailByOrderHeaderId(id);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
           
        }
    }
}
