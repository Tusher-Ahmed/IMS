using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModel
{
    public class GarmentsEditViewModel
    {
        [Required(ErrorMessage = "SKU is required")]
        public List<string> SelectedSKUs { get; set; }

        public GarmentsProduct GarmentsProduct { get; set; }

    }
}
