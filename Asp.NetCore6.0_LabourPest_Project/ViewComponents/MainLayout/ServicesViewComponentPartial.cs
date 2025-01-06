using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class ServicesViewComponentPartial:ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
