using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class ServicesViewComponentPartial:ViewComponent
	{
		ServicesManager servicesManager=new(new EfServicesRepository());
		public IViewComponentResult Invoke()
		{
			var values = servicesManager.GetAll();
			return View(values);
		}
	}
}
