using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class PageSliderViewComponentPartial:ViewComponent
	{
		HomePageManager homePageManager = new(new EfHomePageRepository());
		public IViewComponentResult Invoke()
		{
			var values = homePageManager.GetAll();
			return View(values);
		}
	}
}
