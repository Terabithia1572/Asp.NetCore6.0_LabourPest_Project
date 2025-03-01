using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class RegisterCustomerController : Controller
    {
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RegisterCustomerController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        private void PopulateCoverOptions2()
        {
            ViewBag.CoverOptions2 = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Müşteri", Text = "Müşteri" }
            };
        }

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
            PopulateCoverOptions2();
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomer(Writer writer)
        {
            if (writer.WriterAbout == "Admin")
            {
                writer.WriterAbout = "Admin";
            }
            else if (writer.WriterAbout == "Müşteri")
            {
                writer.WriterAbout = "Müşteri";
            }
            writer.WriterStatus = true;
            writerManager.TAdd(writer);

            // WriterMail alanına göre klasör adı oluşturun.
            // (Eğer klasör ismini aynen kullanmak istiyorsanız; aksi halde sanitize edebilirsiniz)
            string folderName = writer.WriterMail; // örneğin "yunus5@gmail.com"
            // Eğer dosya sisteminde özel karakterlerden (boşluk, vb.) kaçınmak isterseniz:
            // string folderName = SanitizeFolderName(writer.WriterMail);

            // wwwroot/labourpestcustomer altında writer'a özel klasörü oluşturun.
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return RedirectToAction("CustomerList", "RegisterCustomer");
        }

        public IActionResult DeleteCustomer(int id)
        {
            var values = writerManager.TGetByID(id);
            writerManager.TDelete(values);
            return RedirectToAction("CustomerList","RegisterCustomer");
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
                // Klasör: wwwroot/labourpestcustomer/customerImage
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "customerImage");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Benzersiz dosya ismi oluştur.
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

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
                // Klasör: wwwroot/labourpestcustomer/AddcustomerImage
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "AddcustomerImage");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string relativePath = $"/labourpestcustomer/AddcustomerImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }

        // İsteğe bağlı: klasör adını sanitize eden yardımcı metot
        private string SanitizeFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return "default_folder";
            return folderName.Replace("@", "_").Replace(" ", "").Replace(".", "_");
        }
    }
}
