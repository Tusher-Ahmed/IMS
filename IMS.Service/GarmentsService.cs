using IMS.DAO;
using IMS.Models;
using IMS.Models.ViewModel;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IMS.Service
{
    public interface IGarmentsService
    {
        IEnumerable<GarmentsProduct> GetAllP();
        GarmentsProductViewModel GetAllProduct(int pageNumber);
        GarmentsProduct GetGarmentsProductById(long id);
        void CreateGarmentsProduct(GarmentsProduct garmentsProduct);
        void UpdateGarmentsProduct(long id, GarmentsProduct garmentsProduct);
        void UpdateStatus(long id);
    }
    public class GarmentsService : IGarmentsService
    {
        private readonly BaseDAO<GarmentsProduct> _repository;
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }

        public GarmentsService()
        {
            _repository = new BaseDAO<GarmentsProduct>();
        }
        #region Get All Product
        public IEnumerable<GarmentsProduct> GetAllP()
        {
            return _repository.GetAll().ToList();
        }
        #endregion

        #region Get All Product with Page number
        public GarmentsProductViewModel GetAllProduct(int pageNumber)
        {
            var query = _repository.GetAll().Where(u=>u.Status==1).ToList();

            int pageSize = 10;
            int totalCount = query.Count();

            IEnumerable<GarmentsProduct> products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var resultModel = new GarmentsProductViewModel
            {
                GarmentsProducts = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return resultModel;
        }
        #endregion

        #region Create Garments Product
        public void CreateGarmentsProduct(GarmentsProduct model)
        {
            int prod = this.GetAllP().Count();
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            using (var transaction = _session.BeginTransaction())
            {
                GarmentsProduct garmentsProduct = new GarmentsProduct
                {
                    Name = model.Name,
                    Image = model.Image,
                    SKU = model.SKU,
                    Price = model.Price,
                    GarmentsId = 1,
                    Description = model.Description,
                    ProductType = model.ProductType,
                    Department = model.Department,
                    ProductCode = prod + 1,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    Status = 1,
                    Rank = highRank + 1,
                    VersionNumber = 1,
                    BusinessId = Guid.NewGuid().ToString()
                };
                try
                {
                    _repository.Add(garmentsProduct);
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

        #region Get Garments Product By Id
        public GarmentsProduct GetGarmentsProductById(long id)
        {
            var product = _repository.GetById(id);
            return product;
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

                    prod.Name = garmentsProduct.Name;
                    prod.SKU = garmentsProduct.SKU;
                    prod.Price = garmentsProduct.Price;
                    prod.Department = garmentsProduct.Department;
                    prod.ProductCode = garmentsProduct.ProductCode;
                    prod.Description = garmentsProduct.Description;
                    prod.VersionNumber = prod.VersionNumber + 1;
                    prod.ModifyBy = 2;
                    prod.ModificationDate = DateTime.Now;

                    try
                    {
                        _repository.Update(prod);
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
        #endregion

        #region Deactivate status
        public void UpdateStatus(long id)
        {
            var prod = this.GetGarmentsProductById(id);
            if (prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    prod.Status = 0;
                    prod.VersionNumber=prod.VersionNumber + 1;
                    prod.ModifyBy = 3;
                    prod.ModificationDate = DateTime.Now;
                    try
                    {
                        _repository.Update(prod);
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
        #endregion
    }
}