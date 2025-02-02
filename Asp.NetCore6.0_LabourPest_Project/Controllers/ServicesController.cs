using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class ServicesController : Controller
    {
        ServicesManager servicesManager = new ServicesManager(new EfServicesRepository());
        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult ServicesList()
        {
            var values = servicesManager.GetAll();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddService()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddService(Services services)
        {
            services.ServicesStatus = true;
            servicesManager.TAdd(services);
            return RedirectToAction("ServicesList", "Services");
        }
        public IActionResult DeleteService(int id)
        {
            var serviceValue = servicesManager.TGetByID(id);
            servicesManager.TDelete(serviceValue);
            return RedirectToAction("ServicesList", "Services");
        }
        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            var serviceValue = servicesManager.TGetByID(id);
            return View(serviceValue);
        }
        [HttpPost]
        public IActionResult UpdateService(Services services)
        {
            services.ServicesStatus = true;
            servicesManager.TUpdate(services);
            return RedirectToAction("ServicesList", "Services");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "servicesImage");
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
                string relativePath = $"/labourpestcustomer/servicesImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }

}
