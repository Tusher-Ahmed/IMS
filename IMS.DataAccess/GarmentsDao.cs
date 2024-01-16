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
    public interface IGarmentsDao
    {
        ISession Session { get; set; }
        GarmentsProductViewModel GetAllProduct(GarmentsProductViewModel garmentsProduct, long? supplierId);
        GarmentsProduct GetGarmentsProductBySupplierId(long id, long userId);
        GarmentsProduct GetGarmentsProductByProductCode(int id);
        List<GarmentsProduct> GetAllPro(int status, long userId);

    }
    public class GarmentsDao:BaseDAO<GarmentsProduct>,IGarmentsDao
    {
        public new ISession Session { get; set; }

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

        public GarmentsProduct GetGarmentsProductByProductCode(int id)
        {
            return Session.Query<GarmentsProduct>().FirstOrDefault(x => x.ProductCode == id);
        }

        public GarmentsProduct GetGarmentsProductBySupplierId(long id, long userId)
        {
            return Session.Query<GarmentsProduct>().Where(u => u.Id == id && u.GarmentsId == userId).FirstOrDefault();
        }
    }
}
