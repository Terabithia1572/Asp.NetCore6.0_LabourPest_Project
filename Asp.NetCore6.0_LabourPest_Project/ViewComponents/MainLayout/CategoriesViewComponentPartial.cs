using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class CategoriesViewComponentPartial:ViewComponent
	{
		CategoryManager categoryManager= new CategoryManager(new EfCategoryRepository());
        public IViewComponentResult Invoke()
		{
			var values=categoryManager.GetAll();
            return View(values);
		}
	}
}
