using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImageURL { get; set; }
        public bool ProductStatus { get; set; }
        public int CategoryID { get; set; } //ilişkili tabloda
        ////////ilişkili tabloda tutulacak ID
        public Category Category { get; set; }
    }
}
