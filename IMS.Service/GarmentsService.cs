﻿using IMS.DAO;
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
        GarmentsProductViewModel GetAllProduct(GarmentsProductViewModel garmentsProduct,long? supplierId);
        GarmentsProduct GetGarmentsProductById(long id);
        GarmentsProduct GetGarmentsProductBySupplierId(long id, long userId);
        GarmentsProduct GetGarmentsProductByProductCode(int id);
        void CreateGarmentsProduct(GarmentsProduct garmentsProduct);
       
        void UpdateGarmentsProduct(long id, GarmentsProduct garmentsProduct);
        void UpdateStatus(long id,GarmentsProduct prod);
        void ActivateStatus(long id, GarmentsProduct product);
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
        public GarmentsProductViewModel GetAllProduct(GarmentsProductViewModel garmentsProduct, long? supplierId)
        {
            string condition = string.Empty;
            if (supplierId != 0)
            {
               // query = query.Where(u => u.GarmentsId == supplierId).ToList();
                condition = $" AND G.GarmentsId = '{supplierId}'";
            }
            int pageNumber = 0;
            if (garmentsProduct.PageNumber == 0)
            {
                pageNumber = 1;
            }
            else
            {
                pageNumber = garmentsProduct.PageNumber;
            }

            int pageSize = 12;

            //var query = _repository.GetAll().Where(u=>u.IsPriceAdded==true && u.Status==1);

            if (garmentsProduct.SearchProductTypeId.HasValue)
            {
                condition += $" AND G.ProductTypeId = '{garmentsProduct.SearchProductTypeId.Value}'";
            }

            if (garmentsProduct.SearchDepartmentId.HasValue)
            {
                condition += $" AND G.DepartmentId = '{garmentsProduct.SearchDepartmentId.Value}'";
            }

            if (!string.IsNullOrWhiteSpace(garmentsProduct.SearchProductName))
            {
                condition += $" AND G.Name LIKE '%{garmentsProduct.SearchProductName}%' ";
            }

            var re = $@"
SELECT *
FROM GarmentsProduct AS G
WHERE G.Status = '1'
{condition}
";
            var iquery = Session.CreateSQLQuery(re);
            iquery.AddEntity(typeof(GarmentsProduct));
            var query = iquery.List<GarmentsProduct>().ToList();
           
            int totalCount = query.Count();

            var products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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
            //int prod = this.GetAllP().Count();
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            int prodCode= Convert.ToInt32(_repository.GetAll().Max(u => u.ProductCode));
            using (var transaction = _session.BeginTransaction())
            {
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
        public GarmentsProduct GetGarmentsProductBySupplierId(long id,long userId)
        {
           return Session.Query<GarmentsProduct>().Where(u=>u.Id==id && u.GarmentsId==userId).FirstOrDefault();
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
                    prod.Image = garmentsProduct.Image;
                    prod.Department = garmentsProduct.Department;
                    prod.ProductType = garmentsProduct.ProductType;
                    prod.ProductCode = prod.ProductCode;
                    prod.Description = garmentsProduct.Description;
                    prod.VersionNumber = prod.VersionNumber + 1;
                    prod.ModifyBy = garmentsProduct.ModifyBy;
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
        public void UpdateStatus(long id, GarmentsProduct product)
        {
            var prod = this.GetGarmentsProductById(id);
            if (prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    prod.Status = 0;
                    prod.VersionNumber=prod.VersionNumber + 1;
                    prod.ModifyBy = product.ModifyBy;
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

        #region Activate Status
        public void ActivateStatus(long id, GarmentsProduct product)
        {
            var prod = GetGarmentsProductById(id);
            if(prod != null)
            {
                using (var transaction = _session.BeginTransaction())
                {
                    prod.Status = 1;
                    prod.VersionNumber = prod.VersionNumber + 1;
                    prod.ModifyBy = product.ModifyBy;
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

        public GarmentsProduct GetGarmentsProductByProductCode(int id)
        {
            return _session.Query<GarmentsProduct>().FirstOrDefault(x=>x.ProductCode == id);
        }

        public List<GarmentsProduct> GetAllPro(int status, long userId)
        {
            string query = $@"
SELECT *
FROM GarmentsProduct AS GP
WHERE GP.Status = '{status}' AND GP.GarmentsId = '{userId}'
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(GarmentsProduct));
            var res = iquery.List<GarmentsProduct>().ToList();

            return res;
        }        

    }
}