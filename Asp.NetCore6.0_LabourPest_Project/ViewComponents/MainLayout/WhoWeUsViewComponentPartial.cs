using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class WhoWeUsViewComponentPartial:ViewComponent
	{
		WhoWeUsManager whoWeUSManager = new(new EfWhoWeUsRepository());
        public IViewComponentResult Invoke()
		{
			var values = whoWeUSManager.GetAll();
			return View(values);
		}
	}
}
