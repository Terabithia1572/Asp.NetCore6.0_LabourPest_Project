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
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "Image");
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
                string relativePath = $"/labourpestcustomer/Image/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
