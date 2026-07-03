using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class BlogCategory
    {
        [Key]
        public int BlogCategoryID { get; set; }

        public string BlogCategories { get; set; }
        public bool BlogCategoryStatus { get; set; }

        // Bu kategoriye ait bloglar (1-n ilişki)
        // ✅ Bu niteliği eklediğin an Swagger ve API çıktısında bu alan tamamen yok olur!
        [JsonIgnore]
        public List<Blog> Blogs { get; set; }


    }
}
