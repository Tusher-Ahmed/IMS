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
        IEnumerable<OrderHeader> GetAllOrderHeaders();
        void AddOrderHeader(OrderHeader orderHeader);
        void Update(OrderHeader orderHeader);
        void UpdateStatus(long id, string orderStatus, string PaymentStatus=null);
        void UpdateStripeSessionAndIntent(long id, string sessionId,string paymentIntentId);
        List<OrderHeader> GetSellingReports(DateTime? start = null, DateTime? end = null, string searchText="");
        List<OrderHeader> GetAllOrderHeadersWithCondition(string orderStatus = "", string paymentStatus = "");
        List<OrderHeader> GetOrderByStatus(string status = "All");
    }
    public class OrderHeaderService:IOrderHeaderService
    {
        private readonly BaseDAO<OrderHeader> _repository;
        private readonly ISellingReportDAO _sellingReportDAO;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _sellingReportDAO.Session = value; }
        }
        public OrderHeaderService()
        {
            _repository = new BaseDAO<OrderHeader>();
            _sellingReportDAO = new SellingReportDAO();
        }

        #region Add Order Header
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region Update Status
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
        #endregion

        #region Update Stripe Session And Intent
        public void UpdateStripeSessionAndIntent(long id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _repository.GetById(id);
            if (orderFromDb != null)
            {
                orderFromDb.SessionId = sessionId;
                orderFromDb.PaymentIntentId= paymentIntentId;
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
        #endregion

        #region Get Order By Id
        public OrderHeader GetOrderHeaderById(long id)
        {
            return _repository.GetById(id);
        }
        #endregion

        #region Get All Order Headers
        public IEnumerable<OrderHeader> GetAllOrderHeaders()
        {
            return _repository.GetAll();
        }
        #endregion

        #region Get Selling Reports
        public List<OrderHeader> GetSellingReports(DateTime? start = null, DateTime? end = null, string searchText = "")
        {
            return _sellingReportDAO.SellingRecords(start, end, searchText);
        }
        #endregion

        #region GetAllOrderHeadersWithCondition
        public List<OrderHeader> GetAllOrderHeadersWithCondition(string orderStatus = "", string paymentStatus = "")
        {
            string condition = string.Empty;
            if (!string.IsNullOrEmpty(orderStatus))
            {
                condition += $" OH.OrderStatus = '{orderStatus}'";
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                condition += $" AND OH.PaymentStatus = '{paymentStatus}'";
            }
            string res = $@"
SELECT * 
FROM OrderHeader AS OH 
WHERE {condition}
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(OrderHeader));  
            var result=iquery.List<OrderHeader>().ToList();

            return result;
        }
        #endregion

        #region GetOrderByStatus
        public List<OrderHeader> GetOrderByStatus(string status = "All")
        {
            string condition=string.Empty;

            if (status == "Approved")
            {
                condition += $" OH.OrderStatus = 'Approved'";
            }
            else if (status == "Shipped")
            {
                condition += $" OH.OrderStatus = 'Shipped'";
            }
            else if (status == "InProcess")
            {
                condition += $" OH.OrderStatus = 'InProcess'";
            }
            else if (status == "Delivered")
            {
                condition += $" OH.OrderStatus = 'Delivered'";
            }
            else if (status == "Cancelled")
            {
                condition += $" OH.OrderStatus = 'Cancelled' AND OH.PaymentStatus <> 'Refunded'";
            }
            else if (status == "Refunded")
            {
                condition += $" OH.OrderStatus = 'Cancelled' AND OH.PaymentStatus = 'Refunded'";
            }
            else if (status == "All")
            {
                condition += $" OH.OrderStatus IS NOT NULL ";
            }

            string res = $@"
SELECT * 
FROM OrderHeader AS OH 
WHERE {condition}
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(OrderHeader));
            var result = iquery.List<OrderHeader>().ToList();

            return result;
        }
        #endregion
    }
}
