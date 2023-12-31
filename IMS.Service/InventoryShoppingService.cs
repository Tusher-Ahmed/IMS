﻿using IMS.DAO;
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
        IEnumerable<InventoryOrderCart> GetAllInventoryOrders();
        InventoryOrderCart GetproductById(long id, long userId);
        int IncrementCount(InventoryOrderCart inventoryOrderCart, int count);
        int DecrementCount(InventoryOrderCart inventoryOrderCart, int count);
        void RemoveProduct(InventoryOrderCart Cart);
        void RemoveProductFromList(InventoryOrderCart Cart);
    }
    public class InventoryShoppingService:IInventoryShoppingService
    {
        private readonly BaseDAO<InventoryOrderCart> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }

        public InventoryShoppingService()
        {
            _repository = new BaseDAO<InventoryOrderCart>();
        }


        #region Add Inventory Shopping Cart
        public void AddInventoryShoppingCart(InventoryOrderCart inventoryOrderCart)
        {
            
            inventoryOrderCart.GarmentsId = inventoryOrderCart.GarmentsProduct.GarmentsId; // Correctly set GarmentsId based on the selected ProductId


            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var Existproduct = _session.Query<InventoryOrderCart>()
                           .FirstOrDefault(cart => cart.EmployeeId == inventoryOrderCart.EmployeeId && cart.GarmentsProduct.Id==inventoryOrderCart.ProductId );

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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        public InventoryOrderCart GetproductById(long id, long userId)
        {
            return Session.Query<InventoryOrderCart>().Where(u=>u.Id==id && u.EmployeeId==userId).FirstOrDefault();
        }

        public IEnumerable<InventoryOrderCart> GetAllInventoryOrders()
        {
            return _repository.GetAll();
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public int IncrementProductCount(InventoryOrderCart inventoryOrderCart, int count)
        {
            inventoryOrderCart.Count += count;
           _repository.Update(inventoryOrderCart);
            return inventoryOrderCart.Count;
        }

        public int IncrementCount(InventoryOrderCart inventoryOrderCart, int count)
        {
            inventoryOrderCart.Count += count;
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(inventoryOrderCart);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
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
                    catch
                    {
                        transaction.Rollback();
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
