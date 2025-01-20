using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminHomePageController : Controller
    {
        HomePageManager homePageManager = new HomePageManager(new EfHomePageRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminHomePageList()
        {
            var values = homePageManager.GetAll();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddHomePage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddHomePage(HomePage homePage)
        {
            homePage.HomePageStatus = true;
            homePageManager.TAdd(homePage);
            return RedirectToAction("AdminHomePageList", "AdminHomePage");
        }
      
        [HttpGet]
        public IActionResult UpdateHomePage(int id)
        {
            var values = homePageManager.TGetByID(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateHomePage(HomePage homePage)
        {
            homePage.HomePageStatus = true;
            homePageManager.TUpdate(homePage);
            return RedirectToAction("AdminHomePageList", "AdminHomePage");
        }

       public IActionResult DeleteHomePage(int id)
        {
            var values= homePageManager.TGetByID(id);
            homePageManager.TDelete(values);
            return RedirectToAction("AdminHomePageList", "AdminHomePage");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "homePageImage");
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
                string relativePath = $"/labourpestcustomer/homePageImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
