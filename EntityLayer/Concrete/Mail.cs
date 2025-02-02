using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Mail
    {
        public int MailID { get; set; }
        public string ReceiverMail { get; set; }
        public string SenderMail { get; set; } // Gönderen mail adresi
        public string MailName { get; set; }
        public string MailSurname { get; set; }
        public string MailTitle { get; set; }
        public string MailContent { get; set; }
        public DateTime MailDate { get; set; }
    }
}
