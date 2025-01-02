using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurName { get; set; }
        public string EmployeeImage { get; set; }
        public string EmployeeTwitter { get; set; }
        public string EmployeeFacebook { get; set; }
        public string EmployeeInstagram { get; set; }
        public string EmployeeLinkedin { get; set; }
        public bool EmployeeStatus { get; set; }
    }
}
