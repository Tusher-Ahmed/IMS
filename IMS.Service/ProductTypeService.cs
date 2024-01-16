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
    public interface IProductTypeService
    {
        IEnumerable<ProductType> GetAllType();
        void AddProductType(ProductType pType);
        ProductType GetProductTypeById(long id);
        void UpdateProductType(long id, ProductType pType);
        void DeleteProductType(long id);
        List<ProductType> LoadProductTypes(string name);
        List<ProductType> LoadProductTypeThroughStatus(int status);
        List<ProductType> SearchProductType(string text, int status);
    }
    public class ProductTypeService : IProductTypeService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<ProductType> _repository;
        private readonly IProductTypeDao _productTypeDao;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _productTypeDao.Session = value; }
        }
        public ProductTypeService()
        {
            _repository = new BaseDAO<ProductType>();
            _productTypeDao = new ProductTypeDao();
        }

        #region Get All Product Type
        public IEnumerable<ProductType> GetAllType()
        {
            try
            {
                return _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region LoadProductTypes
        public List<ProductType> LoadProductTypes(string name)
        {
            try
            {
                return _productTypeDao.LoadProductTypes(name);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region LoadProductTypeThroughStatus
        public List<ProductType> LoadProductTypeThroughStatus(int status)
        {
            try
            {
                return _productTypeDao.LoadProductTypeThroughStatus(status);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Add Product Type
        public void AddProductType(ProductType pType)
        {            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
                    ProductType productType = new ProductType
                    {
                        Name = pType.Name,
                        CreatedBy = pType.CreatedBy,
                        CreationDate = DateTime.Now,
                        Status = 1,
                        VersionNumber = 1,
                        Rank = highRank + 1,
                        BusinessId = Guid.NewGuid().ToString()
                    };

                    _repository.Add(productType);
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

        #region Search Product Type
        public List<ProductType> SearchProductType(string text, int status)
        {
            try
            {
                return _productTypeDao.SearchProductType(text, status);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Get Product Type By Id
        public ProductType GetProductTypeById(long id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region Update Product Type
        public void UpdateProductType(long id, ProductType pType)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var ProductTypeData = _repository.GetById(id);
                    if (ProductTypeData != null)
                    {
                        ProductTypeData.Name = pType.Name;
                        ProductTypeData.ModifyBy = pType.ModifyBy;
                        ProductTypeData.Status = pType.Status;
                        ProductTypeData.ModificationDate = DateTime.Now;
                        ProductTypeData.VersionNumber = ProductTypeData.VersionNumber + 1;
                    }

                    _repository.Update(ProductTypeData);
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

        #region Delete Product Type
        public void DeleteProductType(long id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var data = _repository.GetById(id);
                    if (data != null)
                    {
                        _repository.Delete(data);
                        transaction.Commit();

                    }
                }
                catch(Exception ex)
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
