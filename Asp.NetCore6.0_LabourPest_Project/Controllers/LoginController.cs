using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class LoginController : Controller
    {
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIN()
        {
            return View();
        }
        public IActionResult SignUP()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterCustomer(Writer writer)
        {
            writer.WriterStatus = true;
            writer.WriterAbout = "Müşteri";
            writerManager.TAdd(writer);
            return RedirectToAction("Deneme","Home");
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "registerCustomer");
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
                string relativePath = $"/labourpestcustomer/registerCustomer/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
