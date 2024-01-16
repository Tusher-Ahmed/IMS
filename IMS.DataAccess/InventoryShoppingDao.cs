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
    public interface IInventoryShoppingDao
    {
        ISession Session { get; set; }
        InventoryOrderCart IsExist(long empId,long productId);
        InventoryOrderCart GetproductById(long id, long userId);
        List<InventoryOrderCart> LoadAllInventoryOrders(long userId);
    }
    public class InventoryShoppingDao:BaseDAO<InventoryOrderCart>, IInventoryShoppingDao
    {
        public new ISession Session { get; set; }

        public InventoryOrderCart GetproductById(long id, long userId)
        {
            return Session.Query<InventoryOrderCart>().Where(u => u.Id == id && u.EmployeeId == userId).FirstOrDefault();
        }

        public InventoryOrderCart IsExist(long empId, long productId)
        {
            return Session.Query<InventoryOrderCart>()
                           .FirstOrDefault(cart => cart.EmployeeId == empId && cart.GarmentsProduct.Id == productId);
        }

        public List<InventoryOrderCart> LoadAllInventoryOrders(long userId)
        {
            string res = $@"
SELECT *
FROM InventoryOrderCart AS IOC
WHERE ( IOC.EmployeeId = {userId})
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(InventoryOrderCart));
            var result = iquery.List<InventoryOrderCart>().ToList();

            return result;
        }
    }
}
