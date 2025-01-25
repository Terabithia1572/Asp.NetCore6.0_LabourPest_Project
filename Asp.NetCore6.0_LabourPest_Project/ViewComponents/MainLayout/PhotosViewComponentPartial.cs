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
            var images = _imageManager.GetAll()
            .OrderByDescending(x => x.ImageID) // En son eklenenler önce gelsin
            .Take(4) // Son 4 veriyi al
            .ToList();

            // Özel sıralama işlemi
            var sortedImages = images
                .OrderBy(x =>
                    x.ImageClass == "col-md-6 corporate" ? 0 : // 1. sırada corporate
                    x.ImageClass == "col-md-6 entertainment innovations" ? 1 : // 2. sırada entertainment
                    2 // 3. ve 4. sırada col-md-3
                )
                .ToList();

            return View(sortedImages);
        }
	}
}
