using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class ImageController : Controller
    {
        ImageManager imageManager = new ImageManager(new EfImagesRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ImageList()
        {
            var values = imageManager.GetAll();
           
            return View(values);
        }
        [HttpGet]
        public IActionResult AddImage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddImage(Image image)
        {
            //string[] imageClasses = { "col-md-6 corporate", "col-md-6 entertainment innovations", "col-md-3" };
            //var images = imageManager.GetAll();
            //int imageCount = images.Count;
            //image.ImageClass = imageClasses[imageCount % imageClasses.Length];

            //// Resim durumunu ayarla ve kaydet
            //image.ImageStatus = true;
            //imageManager.TAdd(image);

            //return RedirectToAction("ImageList", "Image");
            var allImages = imageManager.GetAll().OrderByDescending(x => x.ImageID).ToList();

            // Son eklenen kaydın ImageClass değerine bak
            if (allImages.Any())
            {
                var lastImageClass = allImages.First().ImageClass;

                // ImageClass değerine göre yeni resim için sınıf ata
                image.ImageClass = lastImageClass switch
                {
                    "col-md-6 corporate" => "col-md-6 entertainment innovations",
                    "col-md-6 entertainment innovations" => "col-md-3",
                    _ => "col-md-6 corporate"
                };
            }
            else
            {
                // İlk resim ekleniyorsa varsayılan değer
                image.ImageClass = "col-md-6 corporate";
            }

            // Resim durumunu aktif olarak işaretle ve kaydet
            image.ImageStatus = true;
            imageManager.TAdd(image);

            return RedirectToAction("ImageList", "Image");
        }
        public IActionResult DeleteImage(int id)
        {
            var imageValue = imageManager.TGetByID(id);
            imageManager.TDelete(imageValue);
            return RedirectToAction("ImageList", "Image");
        }
        [HttpGet]
        public IActionResult UpdateImage(int id)
        {
            var imageValue = imageManager.TGetByID(id);
            return View(imageValue);
        }
        [HttpPost]
        public IActionResult UpdateImage(Image image)
        {
            image.ImageStatus = true;
            imageManager.TUpdate(image);
            return RedirectToAction("ImageList", "Image");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "Image2");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Dosya ismini oluştur
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Dosya yolunu oluştur
                string filePath = Path.Combine(folderPath, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Dosya yolunu döndür
                string relativePath = $"/labourpestcustomer/Image2/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
