using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class WhoWeUs
    {
        [Key]
        public int WhoWeUsID { get; set; }
        public string WhoWeUsTitle1 { get; set; }
        public string WhoWeUsTitle2 { get; set; }
        public string WhoWeUsDescription1 { get; set; }
        public string WhoWeUsDescription2 { get; set; }
        public string WhoWeUsImageURL { get; set; }
        public string WhoWeUsClass { get; set; }
        public bool WhoWeUsStatus { get; set; }
    }
}
