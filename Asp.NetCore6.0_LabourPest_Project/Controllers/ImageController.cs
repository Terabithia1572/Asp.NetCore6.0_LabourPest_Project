using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var allImages = imageManager.GetAll().OrderBy(x => x.ImageID).ToList(); // Eskiden OrderByDescending kullanıyorduk

            // Tam sıralı döngü
            string[] imageClasses = { "col-md-6 corporate", "col-md-6 entertainment innovations", "col-md-3", "col-md-3", "col-md-6 corporate" };

            // Eklenen toplam resim sayısını al
            int imageCount = allImages.Count;

            // Yeni resme atanacak indexi belirle
            int nextIndex = imageCount % imageClasses.Length;

            // Yeni sınıfı ata
            image.ImageClass = imageClasses[nextIndex];

            // Resim durumunu aktif olarak işaretle ve kaydet
            image.ImageStatus = true;
            imageManager.TAdd(image);

            return RedirectToAction("ImageList", "Image");
        }


        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var imageValue = imageManager.TGetByID(id);
            if (imageValue == null)
            {
                return NotFound();
            }

            imageManager.TDelete(imageValue);
            return Json(new { success = true });
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
