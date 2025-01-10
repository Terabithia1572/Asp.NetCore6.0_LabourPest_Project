using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class TestimonialsViewComponentPartial:ViewComponent
	{
		CommentManager commentManager = new(new EfCommentRepository()); 
		public IViewComponentResult Invoke()
		{
			var values=commentManager.GetAll();
            return View(values);
		}
	}
}
