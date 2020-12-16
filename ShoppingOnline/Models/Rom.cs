using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Rom
    {
        public int? RomId { get; set; }
        public int? Space { get; set; }
        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
