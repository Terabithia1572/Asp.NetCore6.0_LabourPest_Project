using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Services
    {
        [Key]
        public int ServicesID { get; set; }
        public string ServicesTitle { get; set; }
        public string? ServicesSlug { get; set; }

        public string ServicesDescription { get; set; }
        public string? ServicesLongDescription { get; set; }

        public string ServicesImageURL { get; set; }
        public bool ServicesStatus { get; set; }
    }
}
