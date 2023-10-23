using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class UserDetailViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ShopName { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string Thana { get; set; }
        public string PostalCode { get; set; }
        public string ERole { get; set; }
    }
}
