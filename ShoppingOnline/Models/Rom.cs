using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Rom
    {
        public Rom()
        {
            Products = new HashSet<Product>();
        }

        public int RomId { get; set; }
        public int? Space { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
