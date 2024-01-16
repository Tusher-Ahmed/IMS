using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IMS.Service
{
    public interface IInventoryShoppingService
    {
        void AddInventoryShoppingCart(InventoryOrderCart inventoryOrderCart);
        //IEnumerable<InventoryOrderCart> GetAllInventoryOrders();
        InventoryOrderCart GetproductById(long id, long userId);
        int IncrementCount(InventoryOrderCart inventoryOrderCart, int count);
        int DecrementCount(InventoryOrderCart inventoryOrderCart, int count);
        void RemoveProduct(InventoryOrderCart Cart);
        void RemoveProductFromList(InventoryOrderCart Cart);
        List<InventoryOrderCart> LoadAllInventoryOrders(long userId);
    }
    public class InventoryShoppingService:IInventoryShoppingService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<InventoryOrderCart> _repository;
        private readonly IInventoryShoppingDao _inventoryShoppingDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _inventoryShoppingDao.Session = value; }
        }

        public InventoryShoppingService()
        {
            _repository = new BaseDAO<InventoryOrderCart>();
            _inventoryShoppingDao = new InventoryShoppingDao();
        }


        #region Add Inventory Shopping Cart
        public void AddInventoryShoppingCart(InventoryOrderCart inventoryOrderCart)
        {
            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    inventoryOrderCart.GarmentsId = inventoryOrderCart.GarmentsProduct.GarmentsId; // Correctly set GarmentsId based on the selected ProductId
                    var Existproduct = _inventoryShoppingDao.IsExist(inventoryOrderCart.EmployeeId, inventoryOrderCart.ProductId);

                    if (Existproduct != null)
                    {
                        IncrementProductCount(Existproduct, inventoryOrderCart.Count);
                    }
                    else
                    {
                        _repository.Add(inventoryOrderCart);
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
        #endregion

        public InventoryOrderCart GetproductById(long id, long userId)
        {
            try
            {
                return _inventoryShoppingDao.GetproductById(id, userId);
            }catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        //public IEnumerable<InventoryOrderCart> GetAllInventoryOrders()
        //{
        //    try
        //    {
        //        return _repository.GetAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("An error occurred in YourAction.", ex);
        //        throw;
        //    }
            
        //}

        public List<InventoryOrderCart> LoadAllInventoryOrders(long userId)
        {
            try
            {
                return _inventoryShoppingDao.LoadAllInventoryOrders(userId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public void RemoveProductFromList(InventoryOrderCart Cart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Delete(Cart);
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

        public int IncrementProductCount(InventoryOrderCart inventoryOrderCart, int count)
        {
            try
            {
                inventoryOrderCart.Count += count;
                _repository.Update(inventoryOrderCart);
                return inventoryOrderCart.Count;
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public int IncrementCount(InventoryOrderCart inventoryOrderCart, int count)
        {            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    inventoryOrderCart.Count += count;
                    _repository.Update(inventoryOrderCart);
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    log.Error("An error occurred in YourAction.", ex);
                    throw;
                }
            }

            return inventoryOrderCart.Count;
        }

        public int DecrementCount(InventoryOrderCart inventoryOrderCart, int count)
        {
            if (inventoryOrderCart.Count > 1)
            {
                inventoryOrderCart.Count -= count;
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        _repository.Update(inventoryOrderCart);
                        transaction.Commit();
                    }
                    catch( Exception ex)
                    {
                        transaction.Rollback();
                        log.Error("An error occurred in YourAction.", ex);
                        throw;
                    }
                }

            }
                          
            return inventoryOrderCart.Count;
        }

        public void RemoveProduct(InventoryOrderCart Cart)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Delete(Cart);
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
    }
}
