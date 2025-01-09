using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class PhotosViewComponentPartial:ViewComponent
	{
		ImageManager _imageManager = new(new EfImagesRepository());
		public IViewComponentResult Invoke()
		{
			var values=_imageManager.GetAll();
            return View(values);
		}
	}
}
