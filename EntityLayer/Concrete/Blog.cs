using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Blog
    {
        [Key]
        public int BlogID { get; set; }

        public string BlogTitle { get; set; }
        public string SlugUrl { get; set; }
        public string BlogContent { get; set; }
        public string BlogImage { get; set; }
        public DateTime BlogDate { get; set; }
        public bool BlogStatus { get; set; }

        // BlogCategory ile ilişki
        public int BlogCategoryID { get; set; }
        [ForeignKey("BlogCategoryID")]
        public BlogCategory BlogCategory { get; set; }

        // BlogComments ile ilişki (1-n ilişki)
        public List<BlogComment> BlogComments { get; set; }

        // Writer ile ilişki
        public int WriterID { get; set; }
        [ForeignKey("WriterID")]
        public Writer Writer { get; set; }
        public List<BlogTag> BlogTags { get; set; }

    }
}
