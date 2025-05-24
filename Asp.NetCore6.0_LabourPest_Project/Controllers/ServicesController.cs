using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class ServicesController : Controller
    {
        ServicesManager servicesManager = new ServicesManager(new EfServicesRepository());
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ServicesList()
        {
            var values = servicesManager.GetAll();
            return View(values);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddService()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddService(Services services)
        {
            services.ServicesStatus = true;
            services.ServicesSlug = CreateUniqueSlug(services.ServicesTitle);
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
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateService(Services services)
        {
            var existing = servicesManager.TGetByID(services.ServicesID);
            if (existing == null) return NotFound();

            existing.ServicesTitle = services.ServicesTitle;
            existing.ServicesDescription = services.ServicesDescription;
            existing.ServicesLongDescription = services.ServicesLongDescription;
            existing.ServicesImageURL = services.ServicesImageURL;
            existing.ServicesStatus = true;
            existing.ServicesSlug = CreateUniqueSlug(services.ServicesTitle);

            servicesManager.TUpdate(existing);
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
        private string CreateSlug(string phrase)
        {
            var dict = new Dictionary<string, string>
    {
        { "ş", "s" }, { "Ş", "s" },
        { "ı", "i" }, { "İ", "i" },
        { "ğ", "g" }, { "Ğ", "g" },
        { "ü", "u" }, { "Ü", "u" },
        { "ö", "o" }, { "Ö", "o" },
        { "ç", "c" }, { "Ç", "c" }
    };

            foreach (var entry in dict)
            {
                phrase = phrase.Replace(entry.Key, entry.Value);
            }

            phrase = phrase.ToLowerInvariant();
            phrase = Regex.Replace(phrase, @"[^a-z0-9\s-]", "");
            phrase = Regex.Replace(phrase, @"\s+", " ").Trim();
            phrase = Regex.Replace(phrase, @"\s", "-");
            phrase = Regex.Replace(phrase, @"-+", "-");

            return phrase;
        }

        private string CreateUniqueSlug(string title)
        {
            string slug = CreateSlug(title);
            string baseSlug = slug;
            int count = 1;

            while (servicesManager.GetAll().Any(s => s.ServicesSlug == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }

        [AllowAnonymous]
        public IActionResult Detail(string slug)
        {
            var service = servicesManager.GetAll().FirstOrDefault(x => x.ServicesSlug == slug);
            if (service == null)
                return NotFound();

            return View(service);
        }




    }




}
