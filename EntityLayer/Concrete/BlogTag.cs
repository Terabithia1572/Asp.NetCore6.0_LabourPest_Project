using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class BlogTag
    {
        public int BlogID { get; set; }
        public Blog Blogs { get; set; }

        public int TagID { get; set; }
        public Tag Tags { get; set; }
    }
}
