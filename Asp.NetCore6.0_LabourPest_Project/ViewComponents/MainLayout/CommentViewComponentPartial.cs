using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class CommentViewComponentPartial:ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View(new Asp.NetCore6._0_LabourPest_Project.Models.PublicReviewInput());
		}
	}
}
