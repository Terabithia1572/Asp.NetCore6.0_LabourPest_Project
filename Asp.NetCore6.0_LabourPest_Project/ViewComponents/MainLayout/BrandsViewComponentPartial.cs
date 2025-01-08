using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class BrandsViewComponentPartial:ViewComponent
	{
		BrandsManager brandsManager = new BrandsManager(new EfBrandsRepository());
		public IViewComponentResult Invoke()
		{
			var values=brandsManager.GetAll();
			return View(values);
		}
	}
}
