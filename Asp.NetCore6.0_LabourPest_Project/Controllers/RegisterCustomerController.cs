using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	
    public class RegisterCustomerController : Controller
	{
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        public IActionResult Index()
		{
			return View();
		}
		public IActionResult CustomerList()
		{
			var values = writerManager.GetAll();
            return View(values);
		}
		[HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCustomer(Writer writer)
        {
            writer.WriterStatus = true;
            writerManager.TAdd(writer);
            return RedirectToAction("CustomerList", "RegisterCustomer");
        }
        public IActionResult DeleteCustomer(int id)
        {
            var values = writerManager.TGetByID(id);
            writerManager.TDelete(values);
            return View();
        }
        [HttpGet]
        public IActionResult UpdateCustomer(int id)
        {
            var values = writerManager.TGetByID(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(Writer writer)
        {
            writer.WriterStatus = true;
            writerManager.TUpdate(writer);
            return RedirectToAction("CustomerList", "RegisterCustomer");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "customerImage");
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
                string relativePath = $"/labourpestcustomer/customerImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage2(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "AddcustomerImage");
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
                string relativePath = $"/labourpestcustomer/AddcustomerImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }

    }
}
