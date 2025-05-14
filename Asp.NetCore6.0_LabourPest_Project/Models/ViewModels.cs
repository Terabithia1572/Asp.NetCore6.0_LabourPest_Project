using EntityLayer.Concrete;

namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class ProfileViewModel
    {
        public EntityLayer.Concrete.Writer Writer { get; set; }
        public IEnumerable<EntityLayer.Concrete.Blog> Blogs { get; set; }
       
        public List<EntityLayer.Concrete.Image> Images { get; set; }
        public int BlogCount { get; set; }
        public int CommentCount { get; set; }
        public int ImageCount { get; set; }
        public int CategoryCount { get; set; }
        public int UserCount { get; set; }
        public List<BlogComment> BlogComments { get; set; } // yorum yapanlarla birlikte

    }
  
}
