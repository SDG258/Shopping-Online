using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingOnline.Models
{
    public class ManufacturerAndDiscountViewModel
    {
        public IEnumerable<Manufacturer> ManufacturerViewModel { get; set; }
        public IEnumerable<Discount> DiscountViewModel { get; set; }
    }
}
