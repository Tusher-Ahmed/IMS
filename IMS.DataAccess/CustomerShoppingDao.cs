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
    public interface ICustomerShoppingDao
    {
        ISession Session { get; set; }
        ShoppingCart IsProductExist(ShoppingCart shoppingCart);
        ShoppingCart GetById(long id, long userId);
        List<ShoppingCart> GetAllCartOrders(long userId);
    }
    public class CustomerShoppingDao:BaseDAO<ShoppingCart>, ICustomerShoppingDao
    {
        public new ISession Session { get; set; }

        public List<ShoppingCart> GetAllCartOrders(long userId)
        {
            string res = $@"
SELECT *
FROM ShoppingCart AS SC
WHERE ( SC.CustomerId = {userId})
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(ShoppingCart));
            var result = iquery.List<ShoppingCart>().ToList();

            return result;
        }

        public ShoppingCart GetById(long id, long userId)
        {
            return Session.Query<ShoppingCart>().Where(u => u.Id == id && u.CustomerId == userId).FirstOrDefault();
        }

        public ShoppingCart IsProductExist(ShoppingCart shoppingCart)
        {
            return Session.Query<ShoppingCart>().FirstOrDefault(cart => cart.CustomerId == shoppingCart.CustomerId && cart.Product.Id == shoppingCart.ProductId);
        }
    }
}
