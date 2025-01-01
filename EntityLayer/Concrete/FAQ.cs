using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class FAQ
    {
        [Key]
        public int FAQID { get; set; }
        public string FAQTitle { get; set; }
        public string FAQDescription { get; set; }
        public bool FAQStatus { get; set; }
    }
}
