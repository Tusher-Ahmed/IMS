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
    public interface IInventoryOrderHistoryDao
    {
        ISession Session { get; set; }
        IEnumerable<OrderHistory> GetByOrderId(long orderId);
        List<Product> GetAllRejectedOrder(long userId);
    }
    public class InventoryOrderHistoryDao : BaseDAO<OrderHistory>, IInventoryOrderHistoryDao
    {
        public new ISession Session { get; set; }

        public List<Product> GetAllRejectedOrder(long userId)
        {
            string product = $@"
SELECT *
FROM Product AS P
WHERE P.GarmentsId = '{userId}' 
AND P.Rejected = 'True'
";
            var iquery = Session.CreateSQLQuery(product);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public IEnumerable<OrderHistory> GetByOrderId(long orderId)
        {
            return Session.Query<OrderHistory>().Where(u => u.OrderId == orderId);
        }
    }
}
