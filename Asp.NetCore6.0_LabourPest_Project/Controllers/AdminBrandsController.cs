using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminBrandsController : Controller
    {
        BrandsManager brandsManager= new BrandsManager(new EfBrandsRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BrandsList()
        {
            var values=brandsManager.GetAll();
            ViewBag.markaSayisi = brandsManager.GetAll().Count;
            return View(values);
        }

        [HttpGet]
        public IActionResult AddBrand()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBrand(Brands brands)
        {
            brands.BrandsStatus= true;
            brandsManager.TAdd(brands);
            return RedirectToAction("BrandsList","AdminBrands");
        }
        [HttpGet]
        public IActionResult UpdateBrand(int id)
        {
            var values = brandsManager.TGetByID(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateBrand(Brands brands)
        {
            brands.BrandsStatus = true;
            brandsManager.TUpdate(brands);
            return RedirectToAction("BrandsList", "AdminBrands");
        }
        public IActionResult DeleteBrand(int id)
        {
            var values = brandsManager.TGetByID(id);
            brandsManager.TDelete(values);
            return RedirectToAction("BrandsList", "AdminBrands");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "brandsImage");
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
                string relativePath = $"/labourpestcustomer/brandsImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
