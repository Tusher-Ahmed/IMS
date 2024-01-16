using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using IMS.Models.ViewModel;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public interface IProductService
    {
        ProductViewModel GetProducts(ProductViewModel product);
        IEnumerable<Product> GetAllProduct();
        void Add(Product product);
        Product GetProductById(long id);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
        Product GetProductByProductCode(int ProductCode);
        bool GetAllProductByProductCode(int ProductCode);
        List<Product> GetAllApprovedProduct();
        List<Product> GetRejectHistory(DateTime? startDate=null, DateTime? endDate = null);
        List<Product> GetAllShortageProduct();
        List<Product> GetAllNewProduct();
        List<Product> LoadYetApprovedProduct();
        List<Product> LoadAllApprovedProducts();
        List<Product> LoadAllRejectedProducts();
        List<Product> LoadDeactivatedProducts();

    }

    public class ProductService:IProductService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<Product> _repository;
        private ISession _session;
        private readonly IProductDAO _productDAO;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value;_productDAO.Session = value; }
        }
        public ProductService()
        {
            _repository = new BaseDAO<Product>();
            _productDAO = new ProductDAO();
            
        }
        #region Customer Home page
        public ProductViewModel GetProducts(ProductViewModel product)
        {
            try
            {
                return _productDAO.GetProducts(product);
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region Get All Product
        public IEnumerable<Product> GetAllProduct()
        {
            try
            {
                return _repository.GetAll();
              // return _productDAO.GetAllProduct();

            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }            
        }
        #endregion

        public List<Product> LoadYetApprovedProduct()
        {
            try
            {
                return _productDAO.LoadYetApprovedProduct();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public List<Product> LoadAllApprovedProducts()
        {
            try
            {
                return _productDAO.LoadAllApprovedProducts();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public List<Product> LoadDeactivatedProducts()
        {
            try
            {
                return _productDAO.LoadDeactivatedProducts();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        public List<Product> LoadAllRejectedProducts()
        {
            try
            {
                return _productDAO.LoadAllRejectedProducts();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }

        #region Add Product
        public void Add(Product product)
        {
            try
            {
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        _repository.Add(product);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
            }
            
        }
        #endregion

        #region Get Product By Id
        public Product GetProductById(long id)
        {
            try
            {
                return _repository.GetById(id);

            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region GetProductByProductCode
        public Product GetProductByProductCode(int ProductCode)
        {
            try
            {
                return _productDAO.GetProductByProductCode(ProductCode);

            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region UpdateProduct
        public void UpdateProduct(Product product)
        {            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _repository.Update(product);
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

        #region Delete Product
        public void DeleteProduct(Product product)
        {
            try
            {
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        _repository.Delete(product);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
           
        }
        #endregion

        #region GetAllProductByProductCode
        public bool GetAllProductByProductCode(int ProductCode)
        {
            try
            {
                return _productDAO.GetAllProductByProductCode(ProductCode);
            }
            catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
            
        }
        #endregion

        #region GetRejectHistory
        public List<Product> GetRejectHistory(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                return _productDAO.GetRejectHistory(startDate, endDate);
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);                
                throw;
            }
        }
        
        #endregion

        #region GetAllApprovedProduct
        public List<Product> GetAllApprovedProduct()
        {
            try
            {
                return _productDAO.GetApprovedProducts();
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Shortage Product
        public List<Product> GetAllShortageProduct()
        {
            try
            {
                return _productDAO.GetAllShortageProduct();
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region New Product for Set Price
        public List<Product> GetAllNewProduct()
        {
            //GetAllNewProduct().Where(u => u.Approved == true && u.IsPriceAdded == false && u.Status == 0)
            try
            {
                return _productDAO.GetAllNewProduct();
            }catch(Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion
    }
}
