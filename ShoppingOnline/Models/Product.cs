using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string NameProduct { get; set; }
        public int? PictureId { get; set; }
        public int? Price { get; set; }
        public int? DiscountId { get; set; }
        public string Note { get; set; }
        public int? ManufacturerId { get; set; }
        public virtual Discount Discount { get; set; }
        public virtual Picture Picture { get; set; }
    }
}
