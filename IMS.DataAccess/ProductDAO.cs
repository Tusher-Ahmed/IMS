using IMS.DAO;
using IMS.Models;
using IMS.Models.ViewModel;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface IProductDAO
    {
        ISession Session { get; set; }
        ProductViewModel GetProducts(ProductViewModel product);
        List<Product> GetRejectHistory(DateTime? startDate = null, DateTime? endDate = null);
        List<Product> GetApprovedProducts();
        List<Product> GetAllShortageProduct();
        List<Product> GetAllNewProduct();
        Product GetProductByProductCode(int ProductCode);
        bool GetAllProductByProductCode(int ProductCode);
        List<Product> LoadAllProductForManagePrice();
        List<Product> LoadYetApprovedProduct();
        List<Product> LoadAllApprovedProducts();
        List<Product> LoadAllRejectedProducts();
        List<Product> LoadDeactivatedProducts();
    }
    public class ProductDAO : BaseDAO<ProductViewModel>, IProductDAO
    {
        public new ISession Session { get; set; }


        public List<Product> GetAllNewProduct()
        {
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

        public List<Product> LoadAllProductForManagePrice()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE (P.Approved = 'True' AND P.IsPriceAdded = 'False' AND P.Status = '0')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public bool GetAllProductByProductCode(int ProductCode)
        {
            return Session.Query<Product>().Where(u => u.ProductCode == ProductCode).ToList().Any(u => u.Approved == null || (u.IsPriceAdded == false && u.Rejected == null));
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

        public List<Product> GetApprovedProducts()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE (P.Status = '1' AND P.IsPriceAdded = 'True' AND P.Approved = 'True')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public Product GetProductByProductCode(int ProductCode)
        {
            return Session.Query<Product>().FirstOrDefault(u => u.ProductCode == ProductCode && u.Status == 1 && u.IsPriceAdded == true && u.Approved == true);
        }

        public ProductViewModel GetProducts(ProductViewModel product)
        {
            int pageNumber = 0;
            if (product.PageNumber == 0)
            {
                pageNumber = 1;
            }
            else
            {
                pageNumber = product.PageNumber;
            }
            int pageSize = 12;

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

        public List<Product> LoadYetApprovedProduct()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE P.Approved IS NULL
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public List<Product> LoadAllApprovedProducts()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE P.Approved = 'True'
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public List<Product> LoadAllRejectedProducts()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE (P.Approved = 'False' AND P.Rejected = 'True')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }

        public List<Product> LoadDeactivatedProducts()
        {
            string res = $@"
SELECT *
FROM Product AS P
WHERE (P.Approved = 'True' AND P.IsPriceAdded = 'True' AND P.Status = '0')
";
            var iquery = Session.CreateSQLQuery(res);
            iquery.AddEntity(typeof(Product));
            var result = iquery.List<Product>().ToList();

            return result;
        }
    }
}
