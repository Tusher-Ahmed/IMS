using IMS.DAO;
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
    }

    public class ProductService:IProductService
    {
        private readonly BaseDAO<Product> _repository;
        private ISession _session;
        public ISession Session
        {
            get { return _session; }
            set { _session = value; _repository.Session = value; }
        }
        public ProductService()
        {
            _repository = new BaseDAO<Product>();
            
        }
        #region Customer Home page
        public ProductViewModel GetProducts(ProductViewModel product)
        {
            int pageNumber = 1;
            int pageSize =  10; 

            var query = _repository.GetAll().Where(u=>u.IsPriceAdded==true && u.Status==1); 

            
            if (product.SearchProductTypeId.HasValue && product.SearchDepartmentId.HasValue)
            {
                query = query.Where(p => p.ProductType.Id == product.SearchProductTypeId.Value && p.Department.Id==product.SearchDepartmentId.Value);
            }
            if (product.SearchProductTypeId.HasValue)
            {
                query = query.Where(p => p.ProductType.Id == product.SearchProductTypeId.Value);
            }

            if (product.SearchDepartmentId.HasValue)
            {
                query = query.Where(p => p.Department.Id == product.SearchDepartmentId.Value);
            }
            if (!string.IsNullOrWhiteSpace(product.SearchProductName))
            {
                string searchKeyword = product.SearchProductName.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchKeyword));
            }

            int totalCount = query.Count();
            
            IEnumerable<Product> products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

         
            var resultModel = new ProductViewModel
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return resultModel;
        }
        #endregion

        public IEnumerable<Product> GetAllProduct()
        {
            return _repository.GetAll();
        }

        public void Add(Product product)
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
        }
        public Product GetProductById(long id)
        {
            return _repository.GetById(id);
        }
        public Product GetProductByProductCode(int ProductCode)
        {
            return _session.Query<Product>().FirstOrDefault(u => u.ProductCode == ProductCode && u.Status==1 && u.IsPriceAdded==true && u.Approved==true);
        }


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
        public void DeleteProduct(Product product)
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
        }
    }
}
