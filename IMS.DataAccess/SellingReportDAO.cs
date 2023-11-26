using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface ISellingReportDAO
    {
        ISession Session { get; set; }
        List<OrderHeader> SellingRecords(DateTime? start = null, DateTime? end = null, string searchText = "");
    }

    public class SellingReportDAO : BaseDAO<OrderHeader>, ISellingReportDAO
    {
        public new ISession Session { get; set; }

        public List<OrderHeader> SellingRecords(DateTime? start = null, DateTime? end = null, string searchText = "")
        {
            string condition = string.Empty;
            string res = RemoveLeadingZeros(searchText);

            if (start.HasValue)
            {
                condition += $" AND OH.OrderDate >= '{start.Value.ToString("yyyy-MM-dd")}'";
            }

            if (end.HasValue)
            {
                condition += $" AND OH.OrderDate <= '{end.Value.ToString("yyyy-MM-dd 23:59:59.999")}'";
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                condition += $" AND (OH.Id LIKE :searchText OR OH.Name LIKE :searchText) ";
            }

            string query = $@"
SELECT * 
FROM OrderHeader AS OH
WHERE OH.OrderStatus <> 'Cancelled' 
{condition}
";

            var iquery = Session.CreateSQLQuery(query);
            if (string.IsNullOrWhiteSpace(searchText) == false) { iquery.SetParameter("searchText", $"%{res}%"); }
            iquery.AddEntity(typeof(OrderHeader));
            var result = iquery.List<OrderHeader>().ToList();

            return result;
        }
        static string RemoveLeadingZeros(string input)
        {
            return input.TrimStart('0');
        }
    }
}
