using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class BlogController : Controller
    {
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        Context c = new();
		private readonly BlogManager _blogManager;

		public BlogController(BlogManager blogManager)
		{
			_blogManager = blogManager;
		}
		public IActionResult BlogDetails(int id)
		{
			ViewBag.id = id;
			var blog = _blogManager.TGetByID(id); // Blog detayını getiren metot
			if (blog == null)
			{
				return NotFound(); // Blog bulunamazsa 404
			}
			return View(blog);
		}

		public IActionResult Index()
        {
            var values = blogManager.GetBlogListWithBlogCategory();
            return View(values);
        }
      
    }
}
