using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        [StringLength(200)]
        public string NotificationType { get; set; }  // Örn: "Comment", "Message", "Subscribe"

        [Required]
        public string NotificationMessage { get; set; }

        public DateTime NotificationDate { get; set; }

        public bool NotificationStatus { get; set; } // false = okunmadı, true = okundu

        // Bildirimi alan kullanıcı
        public int? WriterID { get; set; }

        [ForeignKey("WriterID")]
        public Writer Writer { get; set; }
        public string? NotificationUrl { get; set; }  // Bildirime tıklandığında yönlendirme yapılacak URL
        public int? SenderWriterID { get; set; } // Bildirimi oluşturan kişi
        [ForeignKey("SenderWriterID")]
        public Writer SenderWriter { get; set; } // Yorum yapan gibi

    }
}
