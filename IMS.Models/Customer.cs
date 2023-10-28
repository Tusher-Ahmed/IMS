using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class Customer
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string City {  get; set; }
        public virtual string StreetAddress { get; set; }
        public virtual string Thana { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual long UserId { get; set; }
    }
}
