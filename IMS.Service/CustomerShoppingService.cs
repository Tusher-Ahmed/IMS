using IMS.DAO;
using IMS.DataAccess;
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
        ShoppingCart GetById(long id, long userId);
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);
        void RemoveProduct(ShoppingCart shoppingCart);
        List<ShoppingCart> GetAllCartOrders(long userId);
    }
    public class CustomerShoppingService : ICustomerShoppingService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<ShoppingCart> _repository;
        private readonly ICustomerShoppingDao _customerShoppingDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _customerShoppingDao.Session = value; }
        }
        public CustomerShoppingService()
        {
            _repository = new BaseDAO<ShoppingCart>();
            _customerShoppingDao = new CustomerShoppingDao();
        }

        #region Add Customer Shopping Cart
        public void AddCutomerShoppingCart(ShoppingCart shoppingCart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var Existproduct = _customerShoppingDao.IsProductExist(shoppingCart);
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
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }


        }
        public int IncrementProductCount(ShoppingCart shoppingCart, int count)
        {
            try
            {
                shoppingCart.Count += count;

                _repository.Update(shoppingCart);

                return shoppingCart.Count;
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region GetAllCartOrders
        public List<ShoppingCart> GetAllCartOrders(long userId)
        {
            try
            {
                return _customerShoppingDao.GetAllCartOrders(userId);

            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Get order By Id
        public ShoppingCart GetById(long id, long userId)
        {
            try
            {
                return _customerShoppingDao.GetById(id, userId);

            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region IncrementCount
        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region DecrementCount
        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region Remove Product From Cart
        public void RemoveProduct(ShoppingCart shoppingCart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Delete(shoppingCart);
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
    }
}
