using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using NHibernate;
using NHibernate.Engine;
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
        OrderHeader GetOrderHeaderByUser(long id, long userId);
        IEnumerable<OrderHeader> GetAllOrderHeaders();
        void AddOrderHeader(OrderHeader orderHeader);
        void Update(OrderHeader orderHeader);
        void UpdateStatus(long id, string orderStatus, string PaymentStatus=null);
        void UpdateStripeSessionAndIntent(long id, string sessionId,string paymentIntentId);
        List<OrderHeader> GetSellingReports(DateTime? start = null, DateTime? end = null, string searchText="");
        List<OrderHeader> GetAllOrderHeadersWithCondition(string orderStatus = "", string paymentStatus = "");
        List<OrderHeader> GetOrderByStatus(string status = "All", long? userId = 0);
        List<OrderHeader> LoadTotalOrders(string orderStatus1, string OrderStatus2);
        List<OrderHeader> LoadNewOrders(string orderStatus1, string orderStatus2, string orderStatus3);
        List<OrderHeader> LoadCancelOrders(string orderStatus, string paymentStatus);
    }
    public class OrderHeaderService:IOrderHeaderService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<OrderHeader> _repository;
        private readonly ISellingReportDAO _sellingReportDAO;
        private readonly IOrderHeaderDao _orderHeaderDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _sellingReportDAO.Session = value;_orderHeaderDao.Session = value; }
        }
        public OrderHeaderService()
        {
            _repository = new BaseDAO<OrderHeader>();
            _sellingReportDAO = new SellingReportDAO();
            _orderHeaderDao = new OrderHeaderDao();
        }

        #region Add Order Header
        public void AddOrderHeader(OrderHeader orderHeader)
        {
            
            using (var transaction = _session.BeginTransaction())
            {                
                try
                {
                    orderHeader.BusinessId = Guid.NewGuid().ToString();
                    orderHeader.VersionNumber = 1;
                    _repository.Add(orderHeader);
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
        #endregion

        #region Update OrderHeader
        public void Update(OrderHeader orderHeader)
        {
            using (var transaction = _session.BeginTransaction())
            {                
                try
                {
                    _repository.Update(orderHeader);
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
        #endregion

        #region Update Status
        public void UpdateStatus(long id, string orderStatus, string PaymentStatus = null)
        {
            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var orderFromDb = _repository.GetById(id);
                    if (orderFromDb != null)
                    {
                        orderFromDb.OrderStatus = orderStatus;
                        if (PaymentStatus != null)
                        {
                            orderFromDb.PaymentStatus = PaymentStatus;
                        }
                    }
                    _repository.Update(orderFromDb);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }
        }
        #endregion

        #region Update Stripe Session And Intent
        public void UpdateStripeSessionAndIntent(long id, string sessionId, string paymentIntentId)
        {
            try
            {
                var orderFromDb = _repository.GetById(id);
                if (orderFromDb != null)
                {
                    orderFromDb.SessionId = sessionId;
                    orderFromDb.PaymentIntentId = paymentIntentId;
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
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
           
        }
        #endregion

        #region Get Order By Id
        public OrderHeader GetOrderHeaderById(long id)
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
        public OrderHeader GetOrderHeaderByUser(long id, long userId)
        {
            try
            {
                return _orderHeaderDao.GetOrderHeaderByUser(id, userId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Get All Order Headers
        public IEnumerable<OrderHeader> GetAllOrderHeaders()
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
        #endregion

        #region LoadTotalOrders
        public List<OrderHeader> LoadTotalOrders(string orderStatus1, string OrderStatus2) 
        {
            try
            {
                return _orderHeaderDao.LoadTotalOrders(orderStatus1, OrderStatus2);       
            }
            catch( Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region LoadNewOrders
        public List<OrderHeader> LoadNewOrders(string orderStatus1, string orderStatus2,string orderStatus3)
        {
            try
            {
                return _orderHeaderDao.LoadNewOrders(orderStatus1, orderStatus2,orderStatus3);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region LoadCancelOrders
        public List<OrderHeader> LoadCancelOrders(string orderStatus, string paymentStatus)
        {
            try
            {
                return _orderHeaderDao.LoadCancelOrders(orderStatus, paymentStatus);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Get Selling Reports
        public List<OrderHeader> GetSellingReports(DateTime? start = null, DateTime? end = null, string searchText = "")
        {
            try
            {
                return _sellingReportDAO.SellingRecords(start, end, searchText);

            }catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region GetAllOrderHeadersWithCondition
        public List<OrderHeader> GetAllOrderHeadersWithCondition(string orderStatus = "", string paymentStatus = "")
        {
            try
            {
                return _orderHeaderDao.GetAllOrderHeadersWithCondition(orderStatus, paymentStatus);
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region GetOrderByStatus
        public List<OrderHeader> GetOrderByStatus(string status = "All", long? userId = 0)
        {
            try
            {
                return _orderHeaderDao.GetOrderByStatus(status, userId);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion
    }
}
