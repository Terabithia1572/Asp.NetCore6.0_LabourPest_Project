using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class ProductViewComponentPartial:ViewComponent
	{
		ProductManager productManager = new(new EfProductRepository());
		public IViewComponentResult Invoke()
		{
			var values = productManager.GetAll();
			return View(values);
		}
	}
}
