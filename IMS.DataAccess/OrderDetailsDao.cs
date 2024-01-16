using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface IOrderDetailsDao
    {
        ISession Session { get; set; }
        List<OrderDetail> LoadAllOrdersDetails(long orderHeaderId);
        OrderDetail GetOrderDetailByOrderHeaderId(long id);
    }
    public class OrderDetailsDao:BaseDAO<OrderDetail>, IOrderDetailsDao
    {
        public new ISession Session { get; set; }

        public OrderDetail GetOrderDetailByOrderHeaderId(long id)
        {
            return Session.Query<OrderDetail>().Where(u => u.OrderHeader.Id == id).FirstOrDefault();
        }

        public List<OrderDetail> LoadAllOrdersDetails(long orderHeaderId)
        {
            string res = $@"
SELECT *
FROM OrderDetail AS OD
WHERE ( OD.OrderHeaderId = {orderHeaderId})
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(OrderDetail));
            var result = iquery.List<OrderDetail>().ToList();

            return result;
        }
    }
}
