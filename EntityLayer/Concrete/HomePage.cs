using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class HomePage
    {
        [Key]
        public int HomePageID { get; set; }
        public string HomePageDescription1 { get; set; }
        public string HomePageDescription2 { get; set; }
        public string HomePageSubDescription { get; set; }
        public string HomePageImage { get; set; }
        public bool HomePageStatus { get; set; }
    }
}
