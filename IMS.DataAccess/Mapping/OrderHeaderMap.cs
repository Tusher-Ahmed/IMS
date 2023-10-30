using FluentNHibernate.Mapping;
using IMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Mapping
{
    public class OrderHeaderMap:ClassMap<OrderHeader>
    {
        public OrderHeaderMap()
        {
            Table("OrderHeader"); 

            Id(x => x.Id);
            Map(x => x.CustomerId);
            Map(x => x.OrderDate);
            Map(x => x.ShippingDate);
            Map(x => x.OrderTotal);
            Map(x => x.OrderStatus);
            Map(x => x.PaymentStatus);
            Map(x => x.TrackingNumber);
            Map(x => x.Carrier);
            Map(x => x.PaymentDate);
            Map(x => x.PaymentDueDate);
            Map(x => x.PaymentIntentId);
            Map(x => x.SessionId);
            Map(x => x.ModifyBy);
            Map(x => x.ModificationDate);
            Map(x => x.VersionNumber);
            Map(x => x.BusinessId);
            Map(x => x.Name);
            Map(x => x.PhoneNumber);
            Map(x => x.City);
            Map(x => x.StreetAddress);
            Map(x => x.Thana);
            Map(x => x.PostalCode);
        }
    }
}
