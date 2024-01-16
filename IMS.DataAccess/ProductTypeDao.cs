using IMS.DAO;
using IMS.Models;
using NHibernate;
using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IMS.DataAccess
{
    public interface IProductTypeDao
    {
        ISession Session { get; set; }
        List<ProductType> LoadProductTypes(string name);
        List<ProductType> LoadProductTypeThroughStatus(int status);
        List<ProductType> SearchProductType(string text, int status);
    }
    public class ProductTypeDao:BaseDAO<ProductType>, IProductTypeDao
    {
        public new ISession Session { get; set; }

        public List<ProductType> LoadProductTypes(string name)
        {
            string query = $@"
SELECT *
FROM ProductType AS PT
WHERE PT.Name Like '{name}' 
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(ProductType));
            var res = iquery.List<ProductType>().ToList();

            return res;
        }

        public List<ProductType> LoadProductTypeThroughStatus(int status)
        {
            string query = $@"
SELECT *
FROM ProductType AS PT
WHERE PT.Status = '{status}' 
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(ProductType));
            var res = iquery.List<ProductType>().ToList();

            return res;
        }

        public List<ProductType> SearchProductType(string text, int status)
        {
            string query = $@"
SELECT *
FROM ProductType AS PT
WHERE (PT.Status = '{status}' AND PT.Name Like '{text}')
";
            var iquery = Session.CreateSQLQuery(query);
            iquery.AddEntity(typeof(ProductType));
            var res = iquery.List<ProductType>().ToList();

            return res;
        }
    }
}
