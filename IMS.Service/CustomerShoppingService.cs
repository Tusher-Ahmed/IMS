using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IMS.Service
{
    public interface ICustomerShoppingService
    {
        void AddCutomerShoppingCart(ShoppingCart shoppingCart);
        IEnumerable<ShoppingCart> GetAllOrders();
        ShoppingCart GetById(long id);
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);
        void RemoveProduct(ShoppingCart shoppingCart);
    }
    public class CustomerShoppingService: ICustomerShoppingService
    {
        private readonly BaseDAO<ShoppingCart> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public CustomerShoppingService()
        {
            _repository=new BaseDAO<ShoppingCart>();
        }

        #region Add Customer Shopping Cart
        public void AddCutomerShoppingCart(ShoppingCart shoppingCart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var Existproduct = _session.Query<ShoppingCart>()
                           .FirstOrDefault(cart => cart.CustomerId == shoppingCart.CustomerId && cart.Product.Id == shoppingCart.ProductId);

                    if (Existproduct != null)
                    {
                        IncrementProductCount(Existproduct, shoppingCart.Count);
                    }
                    else
                    {
                        _repository.Add(shoppingCart);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public int IncrementProductCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(shoppingCart);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
                      
            return shoppingCart.Count;
        }
        #endregion

        public IEnumerable<ShoppingCart> GetAllOrders()
        {
            return _repository.GetAll();
        }
        public ShoppingCart GetById(long id)
        {
            return _repository.GetById(id);
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(shoppingCart);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return shoppingCart.Count;
        }
        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            if (shoppingCart.Count > 1)
            {
                shoppingCart.Count -= count;
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        _repository.Update(shoppingCart);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }

            return shoppingCart.Count;
        }

        public void RemoveProduct(ShoppingCart shoppingCart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Delete(shoppingCart);
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
}
