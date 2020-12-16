using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Ram
    {
        public int RamId { get; set; }
        public int? Memory { get; set; }
        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
