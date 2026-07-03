using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class BlogComment
    {
        [Key]
        public int BlogCommentID { get; set; }

        public string BlogCommentUserName { get; set; }
        public string BlogCommentTitle { get; set; }
        public string BlogCommentContent { get; set; }
        public string BlogImageUrl { get; set; }
        public DateTime BlogCommentDate { get; set; }
        public bool BlogCommentStatus { get; set; }

        // Blog ile ilişki
        public int BlogID { get; set; }
        [ForeignKey("BlogID")]
        [JsonIgnore] // 🎯 Swagger'da görünmesini engeller
        public Blog Blog { get; set; }
        public int? WriterID { get; set; } // null olabilen bir alan
        [JsonIgnore] // 🎯 Swagger'da görünmesini engeller
        public Writer Writer { get; set; } // Writer ile ilişki

        public int? ParentCommentID { get; set; } // 🔁 Yanıt özelliği için eklendi
        [ForeignKey("ParentCommentID")]
        [JsonIgnore] // 🎯 Swagger'da görünmesini engeller
        public BlogComment ParentComment { get; set; } // (İsteğe bağlı navigation)
    }
}
