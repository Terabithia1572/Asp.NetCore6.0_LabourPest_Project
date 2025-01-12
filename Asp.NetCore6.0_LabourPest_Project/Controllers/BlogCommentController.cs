using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	public class BlogCommentController : Controller
	{
		BlogCommentManager blogCommentManager = new BlogCommentManager(new EfBlogCommentRepository());
		public IActionResult Index()
		{
			return View();
		}
		public PartialViewResult PartialAddComment()
		{
			return PartialView();
		}
		public PartialViewResult CommentListByBlog(int id)
		{
			
			var values=blogCommentManager.GetComments(id);
			return PartialView(values);
		}
	}
}
