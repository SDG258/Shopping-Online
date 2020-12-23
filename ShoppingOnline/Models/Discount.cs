﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Products = new HashSet<Product>();
        }

        public string DiscountCode { get; set; }
        public int DiscountId { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Used { get; set; }
        public int? Amount { get; set; }
        public string Note { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
