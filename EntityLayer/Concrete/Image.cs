using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Image
    {
        [Key]
        public int ImageID { get; set; }
        public string ImageUpload { get; set; }
        public bool ImageStatus { get; set; }
    }
}
