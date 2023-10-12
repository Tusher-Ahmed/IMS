using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IInventoryShoppingService
    {
        void AddInventoryShoppingCart(InventoryOrderCart inventoryOrderCart);
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
            inventoryOrderCart.EmployeeId = 1;
            inventoryOrderCart.GarmentsId = inventoryOrderCart.GarmentsProduct.GarmentsId; // Correctly set GarmentsId based on the selected ProductId


            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var Existproduct = _session.Query<InventoryOrderCart>()
                           .FirstOrDefault(cart => cart.EmployeeId == inventoryOrderCart.EmployeeId && cart.GarmentsProduct.Id==inventoryOrderCart.ProductId );

                    if (Existproduct != null)
                    {
                        IncrementCount(Existproduct, inventoryOrderCart.Count);
                        _repository.Update(Existproduct);
                    }
                    else
                    {
                        _repository.Add(inventoryOrderCart);
                    }
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion




        public int IncrementCount(InventoryOrderCart inventoryOrderCart, int count)
        {
            inventoryOrderCart.Count += count;
            return inventoryOrderCart.Count;
        }
    }
}
