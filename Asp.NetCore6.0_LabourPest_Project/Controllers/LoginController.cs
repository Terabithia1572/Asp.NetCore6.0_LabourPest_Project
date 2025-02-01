using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        Context context = new Context();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIN()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIN(Writer writer)
        {
            var datavalue = context.Writers.FirstOrDefault(x => x.WriterMail == writer.WriterMail && x.WriterPassword == writer.WriterPassword);
            if (datavalue != null)
            {
                // Doğru writer ID'sini kullanmak için 'datavalue.WriterID' alınmalı:
                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, datavalue.WriterID.ToString()),
        new Claim(ClaimTypes.Name, datavalue.WriterMail),
        new Claim(ClaimTypes.Role, datavalue.WriterAbout)
    };
                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                // Kullanıcının rolüne göre yönlendirme yapıyoruz.
                if (datavalue.WriterAbout == "Admin")
                {
                    return RedirectToAction("Profile", "AdminDashboard");
                }
                else if (datavalue.WriterAbout == "Müşteri")
                {
                    return RedirectToAction("Test", "Writer");
                }
                else
                {
                    // Diğer durumları burada ele alabilirsiniz.
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }
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
            writer.WriterStatus = false;
            writer.WriterAbout = "Müşteri"; // Kayıt edilen kullanıcının rolü "Müşteri" olarak belirleniyor.
            writerManager.TAdd(writer);
            return RedirectToAction("Deneme", "Home");
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
