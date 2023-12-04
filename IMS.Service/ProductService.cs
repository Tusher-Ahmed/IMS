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
        List<Product> GetAllProductByProductCode(int ProductCode);
        List<Product> GetAllApprovedProduct();
        List<Product> GetRejectHistory(DateTime? startDate=null, DateTime? endDate = null);
        List<Product> GetAllShortageProduct();
        List<Product> GetAllNewProduct();
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
            int pageSize =  12;

            //var query = _repository.GetAll().Where(u=>u.IsPriceAdded==true && u.Status==1);

            string condition = string.Empty;

            if (product.SearchProductTypeId.HasValue)
            {
                condition += $" AND P.ProductTypeId = '{product.SearchProductTypeId.Value}'";
            }

            if (product.SearchDepartmentId.HasValue)
            {
                condition += $" AND P.DepartmentId = '{product.SearchDepartmentId.Value}'";
            }

            if (!string.IsNullOrWhiteSpace(product.SearchProductName))
            {
                condition += $" AND P.Name LIKE '%{product.SearchProductName}%' ";
            }

            var data = $@"
SELECT * 
FROM Product AS P
WHERE P.IsPriceAdded = 'True' AND P.Status = '1'
{condition}
";
            var iquery = Session.CreateSQLQuery(data);
            iquery.AddEntity(typeof(Product));
            var query = iquery.List<Product>().ToList();

            int totalCount = query.Count();
            
            var products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

         
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

        #region Get All Product
        public IEnumerable<Product> GetAllProduct()
        {
            return _repository.GetAll();
        }
        #endregion

        #region Add Product
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
        #endregion

        #region Get Product By Id
        public Product GetProductById(long id)
        {
            return _repository.GetById(id);
        }
        #endregion

        #region GetProductByProductCode
        public Product GetProductByProductCode(int ProductCode)
        {
            return _session.Query<Product>().FirstOrDefault(u => u.ProductCode == ProductCode && u.Status==1 && u.IsPriceAdded==true && u.Approved==true);
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
        #endregion

        #region GetAllProductByProductCode
        public List<Product> GetAllProductByProductCode(int ProductCode)
        {
            return _session.Query<Product>().Where(u=>u.ProductCode== ProductCode).ToList();
        }
        #endregion

        #region GetRejectHistory
        public List<Product> GetRejectHistory(DateTime? startDate = null, DateTime? endDate = null)
        {
            string condition = string.Empty;

            if (startDate.HasValue)
            {
                condition += $" AND P.CreationDate >= '{startDate.Value.ToString("yyyy-MM-dd")}'";
            }

            if (endDate.HasValue)
            {
                condition += $" AND P.CreationDate <= '{endDate.Value.ToString("yyyy-MM-dd 23:59:59.999")}'";
            }
            string product = $@"
SELECT *
FROM Product AS P
WHERE P.Approved = 'False' And P.Rejected = 'True' {condition}
";
            var iquery = Session.CreateSQLQuery(product);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }
        #endregion

        #region GetAllApprovedProduct
        public List<Product> GetAllApprovedProduct()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE (P.Status = '1' AND P.IsPriceAdded = 'True' AND P.Approved = 'True')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result=iquery.List<Product>().ToList();

            return result;
        }

        public List<Product> GetAllShortageProduct()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE ( P.Quantity <= '0' AND P.IsPriceAdded = 'True')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public List<Product> GetAllNewProduct()
        {
            //GetAllNewProduct().Where(u => u.Approved == true && u.IsPriceAdded == false && u.Status == 0)
            string res = $@"
SELECT *
FROM Product AS P
WHERE ( P.Approved = 'True' AND P.IsPriceAdded = 'False' AND P.Status = '0')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        #endregion

    }
}
