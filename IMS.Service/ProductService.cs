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
        public ProductViewModel GetProducts(ProductViewModel product)
        {
            int pageNumber = 1;
            int pageSize =  10; 

            var query = _repository.GetAll(); 

            
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
                query = query.Where(p => p.Name.Contains(product.SearchProductName));
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
    }
}
