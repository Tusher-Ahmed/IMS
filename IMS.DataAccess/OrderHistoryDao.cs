using IMS.DAO;
using IMS.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMS.DataAccess
{
    public interface IOrderHistoryDao
    {
        ISession Session { get; set; }
        List<OrderHistory> GetOrderHistories(List<long> ids, DateTime? startDate = null, DateTime? endDate = null, string searchText = "");
    }

    public class OrderHistoryDao : BaseDAO<OrderHistory>, IOrderHistoryDao
    {
        public new ISession Session { get; set; }

        public List<OrderHistory> GetOrderHistories(List<long> ids, DateTime? startDate = null, DateTime? endDate = null, string searchText = "")
        {
            string condition = string.Empty;
            string res = RemoveLeadingZeros(searchText);

            if (startDate.HasValue)
            {
                condition += $" AND oh.CreationDate >= '{startDate.Value.ToString("yyyy-MM-dd")}'";
            }

            if (endDate.HasValue)
            {
                condition += $" AND oh.CreationDate <= '{endDate.Value.ToString("yyyy-MM-dd 23:59:59.999")}'";
            }

            if (!string.IsNullOrEmpty(searchText))
            {                
                condition += $" AND (oh.OrderId LIKE :searchRes OR u.UserName LIKE :searchRes) ";
            }

            string query = $@"
SELECT oh.*
FROM OrderHistory AS oh WITH(NOLOCK) 
INNER JOIN AspNetUsers AS u WITH(NOLOCK) ON u.Id = oh.EmployeeId
WHERE oh.Id IN({string.Join(",", ids)})
{condition}
";

            var iquery = Session.CreateSQLQuery(query);
            if(string.IsNullOrWhiteSpace(searchText) == false) { iquery.SetParameter("searchRes", $"%{res}%"); }
            iquery.AddEntity(typeof(OrderHistory));
            var result = iquery.List<OrderHistory>().ToList();

            return result;
        }
        static string RemoveLeadingZeros(string input)
        {
            return input.TrimStart('0');
        }
    }
}
