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
            string ipAddress = GetClientIpAddress(HttpContext);

            if (datavalue != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, datavalue.WriterID.ToString()),
            new Claim(ClaimTypes.Name, datavalue.WriterMail),
            new Claim(ClaimTypes.Role, datavalue.WriterAbout)
        };
                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                // Başarılı giriş log kaydı
                Log logEntry = new Log
                {
                    UserName = datavalue.WriterMail,
                    Date = DateTime.Now,
                    Action = "Giriş Denemesi",
                    Success = true,
                    IPAddress = ipAddress
                };

                context.Logs.Add(logEntry);
                await context.SaveChangesAsync();

                // Kullanıcının rolüne göre yönlendirme
                if (datavalue.WriterAbout == "Admin")
                {
                    return RedirectToAction("Profile", "AdminDashboard");
                }
                else if (datavalue.WriterAbout == "Müşteri")
                {
                    return RedirectToAction("Profile", "AdminDashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Başarısız giriş log kaydı
                Log logEntry = new Log
                {
                    UserName = writer.WriterMail,
                    Date = DateTime.Now,
                    Action = "Giriş Denemesi", // İsteğe bağlı: "Başarısız Giriş Denemesi"
                    Success = false,
                    IPAddress = ipAddress
                };
                context.Logs.Add(logEntry);
                await context.SaveChangesAsync();

                return View();
            }
        }

        public async Task<IActionResult> LogOut()
        {
            string ipAddress = GetClientIpAddress(HttpContext);
            string userName = User.Identity.Name;

            // Çıkış log kaydı
            Log logEntry = new Log
            {
                UserName = userName,
                Date = DateTime.Now,
                Action = "Çıkış Yaptı",
                Success = true,
                IPAddress = ipAddress
            };

            context.Logs.Add(logEntry);
            await context.SaveChangesAsync();

            await HttpContext.SignOutAsync();
            return RedirectToAction("Deneme", "Home");
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // İlk olarak Forwarded Headers üzerinden gelen değeri deniyoruz:
            string ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            // Eğer Forwarded Header boşsa, doğrudan connection bilgisinden alıyoruz:
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }

            // Yerel geliştirme ortamında IPv6 loopback adresi ::1 ise, IPv4 karşılığı olan 127.0.0.1'e dönüştürüyoruz:
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
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
