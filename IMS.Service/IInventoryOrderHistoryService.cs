using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using IMS.Models.ViewModel;
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
        List<OrderHistory> GetHistories(List<long> ids, DateTime? startDate = null, DateTime? endDate = null, string searchText = "");
        List<Product> GetAllRejectedOrder(long userId);
        OrderHistory GetById(long id);
        List<OrderHistory> LoadHistoryByGarmentsId(long garmentsId);
        List<OrderHistory> LoadTotalHistory(long orderId);
    }
    public class InventoryOrderHistoryService : IInventoryOrderHistoryService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<OrderHistory> _repository;
        private readonly IOrderHistoryDao _orderHistoryDao;
        private readonly IInventoryOrderHistoryDao _inventoryOrderHistoryDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _orderHistoryDao.Session = value;_inventoryOrderHistoryDao.Session = value; }
        }

        public InventoryOrderHistoryService()
        {
            _repository = new BaseDAO<OrderHistory>();
            _orderHistoryDao = new OrderHistoryDao();
            _inventoryOrderHistoryDao = new InventoryOrderHistoryDao();
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
                catch(Exception ex) 
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }
        }

        public IEnumerable<OrderHistory> GetAll()
        {
            try
            {
                return _repository.GetAll();
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }            
        }

        public List<OrderHistory> LoadHistoryByGarmentsId(long garmentsId)
        {
            try
            {
                return _orderHistoryDao.LoadHistoryByGarmentsId(garmentsId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        public IEnumerable<OrderHistory> GetByOrderId(long orderId)
        {
            try
            {
                return _inventoryOrderHistoryDao.GetByOrderId(orderId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        public List<OrderHistory> LoadTotalHistory(long userId)
        {
            try
            {
                return _orderHistoryDao.LoadHistoryByGarmentsId(userId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public OrderHistory GetById(long id)
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

        public List<OrderHistory> GetHistories(List<long> ids, DateTime? startDate = null, DateTime? endDate = null, string searchText = "" )
        {
            try
            {
                return _orderHistoryDao.GetOrderHistories(ids, startDate, endDate, searchText);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public List<Product> GetAllRejectedOrder(long userId)
        {
            try
            {
                return _inventoryOrderHistoryDao.GetAllRejectedOrder(userId);
            }
            catch( Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
    }
}
