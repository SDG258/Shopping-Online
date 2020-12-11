using System;
using System.Collections.Generic;

#nullable disable

namespace ShoppingOnline.Models
{
    public partial class Picture
    {
        public Picture()
        {
            Products = new HashSet<Product>();
        }

        public int PictureId { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
