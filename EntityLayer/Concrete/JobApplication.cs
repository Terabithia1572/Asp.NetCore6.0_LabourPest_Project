using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class JobApplication
    {
        [Key]
        public int JobApplicationID { get; set; }
        public string JobApplicationName { get; set; }
        public string JobApplicationSurName { get; set; }
        public string JobApplicationNumber { get; set; }
        public string JobApplicationMail { get; set; }
        public string JobApplicationCity { get; set; }
        public string JobApplicationCV { get; set; }
        public string JobApplicationAboutYou { get; set; }
        public string JobApplicationStatus { get; set; }
    }
}
