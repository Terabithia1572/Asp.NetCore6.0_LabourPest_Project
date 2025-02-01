namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class ProfileViewModel
    {
        public EntityLayer.Concrete.Writer Writer { get; set; }
        public IEnumerable<EntityLayer.Concrete.Blog> Blogs { get; set; }
    }
}
