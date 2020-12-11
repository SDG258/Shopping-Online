using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class WareHouse
    {
        public int? ProductId { get; set; }
        public int? Cost { get; set; }
        public int? TheRemainingAmount { get; set; }
        public int? QuantityStillEntered { get; set; }
        public DateTime? Date { get; set; }

        public virtual Product Product { get; set; }
    }
}
