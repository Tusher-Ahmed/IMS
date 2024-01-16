using IMS.DAO;
using IMS.DataAccess;
using IMS.Models;
using IMS.Models.ViewModel;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IMS.Service
{
    public interface IGarmentsService
    {
        IEnumerable<GarmentsProduct> GetAllP();
        List<GarmentsProduct> GetAllPro(int status, long userId);
        GarmentsProductViewModel GetAllProduct(GarmentsProductViewModel garmentsProduct, long? supplierId);
        GarmentsProduct GetGarmentsProductById(long id);
        GarmentsProduct GetGarmentsProductBySupplierId(long id, long userId);
        GarmentsProduct GetGarmentsProductByProductCode(int id);
        void CreateGarmentsProduct(GarmentsProduct garmentsProduct);

        void UpdateGarmentsProduct(long id, GarmentsProduct garmentsProduct);
        void UpdateStatus(long id, GarmentsProduct prod);
        void ActivateStatus(long id, GarmentsProduct product);
    }
    public class GarmentsService : IGarmentsService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BaseDAO<GarmentsProduct> _repository;
        private readonly IGarmentsDao _garmentsDao;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; _garmentsDao.Session = value; }
        }

        public GarmentsService()
        {
            _repository = new BaseDAO<GarmentsProduct>();
            _garmentsDao = new GarmentsDao();
        }
        #region Get All Product
        public IEnumerable<GarmentsProduct> GetAllP()
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

        #region Get All Product with Page number

        //TODO : Task: move all querys to repository
        public GarmentsProductViewModel GetAllProduct(GarmentsProductViewModel garmentsProduct, long? supplierId)
        {
            try
            {
                return _garmentsDao.GetAllProduct(garmentsProduct, supplierId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }
        }
        #endregion

        #region Create Garments Product
        public void CreateGarmentsProduct(GarmentsProduct model)
        {
            //int prod = this.GetAllP().Count();            
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
                    int prodCode = Convert.ToInt32(_repository.GetAll().Max(u => u.ProductCode));
                    GarmentsProduct garmentsProduct = new GarmentsProduct
                    {
                        Name = model.Name,
                        Image = model.Image,
                        SKU = model.SKU,
                        Price = model.Price,
                        GarmentsId = model.GarmentsId,
                        Description = model.Description,
                        ProductType = model.ProductType,
                        Department = model.Department,
                        ProductCode = prodCode + 1,
                        CreatedBy = model.GarmentsId,
                        CreationDate = DateTime.Now,
                        Status = 1,
                        Rank = highRank + 1,
                        VersionNumber = 1,
                        BusinessId = Guid.NewGuid().ToString()
                    };

                    _repository.Add(garmentsProduct);
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

        #region Get Garments Product By Id
        public GarmentsProduct GetGarmentsProductById(long id)
        {
            try
            {
                var product = _repository.GetById(id);
                return product;
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        public GarmentsProduct GetGarmentsProductBySupplierId(long id, long userId)
        {
            try
            {
                return _garmentsDao.GetGarmentsProductBySupplierId(id, userId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region Update Garments Product
        public void UpdateGarmentsProduct(long id, GarmentsProduct garmentsProduct)
        {
            var prod = this.GetGarmentsProductById(id);
            if (prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        prod.Name = garmentsProduct.Name;
                        prod.SKU = garmentsProduct.SKU;
                        prod.Price = garmentsProduct.Price;
                        prod.Image = garmentsProduct.Image;
                        prod.Department = garmentsProduct.Department;
                        prod.ProductType = garmentsProduct.ProductType;
                        prod.ProductCode = prod.ProductCode;
                        prod.Description = garmentsProduct.Description;
                        prod.VersionNumber = prod.VersionNumber + 1;
                        prod.ModifyBy = garmentsProduct.ModifyBy;
                        prod.ModificationDate = DateTime.Now;

                        _repository.Update(prod);
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
        #endregion

        #region Deactivate status
        public void UpdateStatus(long id, GarmentsProduct product)
        {
            var prod = this.GetGarmentsProductById(id);
            if (prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        prod.Status = 0;
                        prod.VersionNumber = prod.VersionNumber + 1;
                        prod.ModifyBy = product.ModifyBy;
                        prod.ModificationDate = DateTime.Now;

                        _repository.Update(prod);
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
        #endregion

        #region Activate Status
        public void ActivateStatus(long id, GarmentsProduct product)
        {
            var prod = GetGarmentsProductById(id);
            if (prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        prod.Status = 1;
                        prod.VersionNumber = prod.VersionNumber + 1;
                        prod.ModifyBy = product.ModifyBy;
                        prod.ModificationDate = DateTime.Now;

                        _repository.Update(prod);
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


        #endregion

        #region GetGarmentsProductByProductCode
        public GarmentsProduct GetGarmentsProductByProductCode(int id)
        {
            try
            {
                return _garmentsDao.GetGarmentsProductByProductCode(id);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

        #region Get All Product
        public List<GarmentsProduct> GetAllPro(int status, long userId)
        {
            try
            {
                return _garmentsDao.GetAllPro(status, userId);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred in YourAction.", ex);
                throw;
            }

        }
        #endregion

    }
}