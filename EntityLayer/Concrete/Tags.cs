using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Tag
    {
        public int TagID { get; set; }
        public string TagName { get; set; }

        public List<BlogTag> BlogTags { get; set; }
    }
}
