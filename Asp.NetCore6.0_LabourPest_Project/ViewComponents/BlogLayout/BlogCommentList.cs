using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.BlogLayout
{
	public class BlogCommentList:ViewComponent
	{
		BlogCommentManager blogCommentManager = new BlogCommentManager(new EfBlogCommentRepository());
		public IViewComponentResult Invoke(int id)
		{
			var values=blogCommentManager.GetComments(id);
			return View(values);
		}
	}
}
