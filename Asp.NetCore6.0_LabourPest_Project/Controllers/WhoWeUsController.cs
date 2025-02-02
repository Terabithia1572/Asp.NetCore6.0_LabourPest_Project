using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class WhoWeUsController : Controller
    {
        WhoWeUsManager whoWeUsManager = new(new EfWhoWeUsRepository());
        public IActionResult Index()
        {
            return View();
        }
        // Dropdown seçeneklerini view'a göndermek için yardımcı metot
        private void PopulateCoverOptions()
        {
            ViewBag.CoverOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Resim Sağda", Text = "Resim Sağda" },
                new SelectListItem { Value = "Resim Solda", Text = "Resim Solda" }
            };
        }
        public IActionResult WhoWeUsList()
        {
            var values=whoWeUsManager.GetAll();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddWhoWeUs()
        {
            PopulateCoverOptions();
            return View();
        }
        [HttpPost]
        public IActionResult AddWhoWeUs(WhoWeUs whoWeUs)
        {
            if (whoWeUs.WhoWeUsClass == "Resim Sağda")
            {
                whoWeUs.WhoWeUsClass = "s-cover-right";
            }
            else if (whoWeUs.WhoWeUsClass == "Resim Solda")
            {
                whoWeUs.WhoWeUsClass = "s-cover-left";
            }

            whoWeUs.WhoWeUsStatus = true;
            whoWeUsManager.TAdd(whoWeUs);
            return RedirectToAction("WhoWeUsList", "WhoWeUs");
        }
        [HttpGet]
        public IActionResult UpdateWhoWeUs(int id)
        {
          
            var values = whoWeUsManager.TGetByID(id);
            PopulateCoverOptions();
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateWhoWeUs(WhoWeUs whoWeUs)
        {
            if (whoWeUs.WhoWeUsClass == "Resim Sağda")
            {
                whoWeUs.WhoWeUsClass = "s-cover-right";
            }
            else if (whoWeUs.WhoWeUsClass == "Resim Solda")
            {
                whoWeUs.WhoWeUsClass = "s-cover-left";
            }

            whoWeUs.WhoWeUsStatus = true;
            whoWeUsManager.TUpdate(whoWeUs);
            return RedirectToAction("WhoWeUsList", "WhoWeUs");
        }
        public IActionResult DeleteWhoWeUs(int id)
        {
            var values = whoWeUsManager.TGetByID(id);
            whoWeUsManager.TDelete(values);
            return RedirectToAction("WhoWeUsList", "WhoWeUs");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "whoweusImage");
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
                string relativePath = $"/labourpestcustomer/whoweusImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
