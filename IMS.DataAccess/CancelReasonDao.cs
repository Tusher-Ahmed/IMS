using IMS.DAO;
using IMS.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess
{
    public interface ICancelReasonDao
    {
        ISession Session { get; set; }
        CancelReason GetReasonByOrderHeaderId(long id);
    }
    public class CancelReasonDao:BaseDAO<CancelReason>, ICancelReasonDao
    {
        public new ISession Session { get; set; }

        public CancelReason GetReasonByOrderHeaderId(long id)
        {
            return Session.Query<CancelReason>().Where(u => u.OrderHeader.Id == id).FirstOrDefault();
        }
    }
}
