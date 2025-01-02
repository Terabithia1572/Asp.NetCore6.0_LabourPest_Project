using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Brands
    {
        [Key]
        public int BrandsID { get; set; }
        public string BrandsImage { get; set; }
        public bool BrandsStatus { get; set; }
    }
}
