namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class BlogDetailViewModel
    {
        public EntityLayer.Concrete.Blog Blog { get; set; }
        public EntityLayer.Concrete.Writer Writer { get; set; }
        public List<EntityLayer.Concrete.BlogComment> Comments { get; set; }
    }
}
