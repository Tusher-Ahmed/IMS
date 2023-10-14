using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models
{
    public class InventoryOrderCart
    {
        [Key]
        public virtual long Id { get; set; }
        [Range(1,1000,ErrorMessage ="Please enter a value between 1 and 1000")]
        public virtual int Count {  get; set; }
        public virtual long EmployeeId { get; set; }
        public virtual long GarmentsId {  get; set; }
        public virtual long ProductId {  get; set; }
        public virtual GarmentsProduct GarmentsProduct { get; set; }
  

    }
}
