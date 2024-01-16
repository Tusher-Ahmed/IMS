using IMS.DAO;
using IMS.Models;
using NHibernate;
using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface IOrderHeaderDao
    {
        ISession Session { get; set; }
        OrderHeader GetOrderHeaderByUser(long id, long userId);
        List<OrderHeader> GetAllOrderHeadersWithCondition(string orderStatus = "", string paymentStatus = "");
        List<OrderHeader> GetOrderByStatus(string status = "All", long? userId = 0);
        List<OrderHeader> LoadTotalOrders(string orderStatus1, string OrderStatus2);
        List<OrderHeader> LoadNewOrders(string orderStatus1, string orderStatus2, string orderStatus3);
        List<OrderHeader> LoadCancelOrders(string orderStatus, string paymentStatus);
    }
    public class OrderHeaderDao:BaseDAO<OrderHeader>, IOrderHeaderDao
    {
        public new ISession Session { get; set; }

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
            var result = iquery.List<OrderHeader>().ToList();

            return result;
        }

        public List<OrderHeader> GetOrderByStatus(string status = "All", long? userId = 0)
        {
            string condition = string.Empty;
            if (userId != 0)
            {
                condition += $" OH.CustomerId = '{userId}' AND";
            }

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

        public OrderHeader GetOrderHeaderByUser(long id, long userId)
        {
            return Session.Query<OrderHeader>().Where(u => u.Id == id && u.CustomerId == userId).FirstOrDefault();
        }

        public List<OrderHeader> LoadCancelOrders(string orderStatus, string paymentStatus)
        {
            string query = $@"
SELECT *
FROM OrderHeader AS OH
WHERE OH.OrderStatus = '{orderStatus}' AND  OH.PaymentStatus = '{paymentStatus}'
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(OrderHeader));
            var res = iquery.List<OrderHeader>().ToList();

            return res;
        }

        public List<OrderHeader> LoadNewOrders(string orderStatus1, string orderStatus2, string orderStatus3)
        {
            string query = $@"
SELECT *
FROM OrderHeader AS OH
WHERE OH.OrderStatus <> '{orderStatus1}' AND OH.OrderStatus <> '{orderStatus2}' AND OH.OrderStatus <> '{orderStatus3}'
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(OrderHeader));
            var res = iquery.List<OrderHeader>().ToList();

            return res;
        }

        public List<OrderHeader> LoadTotalOrders(string orderStatus1, string orderStatus2)
        {
            string query = $@"
SELECT *
FROM OrderHeader AS OH
WHERE OH.OrderStatus = '{orderStatus1}' OR OH.OrderStatus = '{orderStatus2}'
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(OrderHeader));
            var res = iquery.List<OrderHeader>().ToList();

            return res;
        }
        
    }
}
