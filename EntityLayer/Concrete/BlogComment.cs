using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
        public Blog Blog { get; set; }
        //public int WriterID { get; set; } //  Eklenen alan
        //public Writer Writer { get; set; } // ✅ Navigation Property
    }
}
