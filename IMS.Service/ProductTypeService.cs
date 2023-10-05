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
    public class ProductTypeService
    {
        private readonly Repository<ProductType> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public ProductTypeService()
        {
            _repository = new Repository<ProductType>();
        }
        public IEnumerable<ProductType> GetAllType()
        {
            return _repository.GetAll().ToList();
        }
        public void AddProductType(ProductType pType)
        {
            int highRank = Convert.ToInt32(_repository.GetAll().Max(u => u.Rank));
            using (var transaction = _session.BeginTransaction())
            {
                ProductType productType = new ProductType
                {
                    Name = pType.Name,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    Status = 1,
                    VersionNumber = 1,
                    Rank = highRank + 1,
                    BusinessId = Guid.NewGuid().ToString()
                };
                try
                {
                    _repository.Add(productType);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public ProductType GetProductTypeById(long id)
        {
            return _repository.GetById(id);
        }

        public void UpdateProductType(long id, ProductType pType)
        {
            using (var transaction = _session.BeginTransaction())
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
                try
                {
                    _repository.Update(ProductTypeData);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public void DeleteProductType(long id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var data = _repository.GetById(id);
                if (data != null)
                {
                    try
                    {
                        _repository.Delete(data);
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
}
