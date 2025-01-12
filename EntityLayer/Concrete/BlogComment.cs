using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int BlogID { get; set; }
        public Blog Blog { get; set; }
    }
}
