using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class CancelReasonMap:ClassMap<CancelReason>
    {
        public CancelReasonMap()
        {
            Table("CancelReason");

            Id(x => x.Id);
            Map(x => x.OrderStatus);
            Map(x => x.PaymentStatus);
            Map(x => x.Reason);

            References(x => x.OrderHeader, "OrderHeaderId").Cascade.None();
        }
    }
}
