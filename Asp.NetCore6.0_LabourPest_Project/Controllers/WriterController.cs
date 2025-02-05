using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class WriterController : Controller
    {
        // Manager'lar
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        Context context = new Context();

        private readonly IWebHostEnvironment _hostingEnvironment;

        // Constructor ile IWebHostEnvironment inject ediliyor
        public WriterController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // Ana sayfa
        public IActionResult Index()
        {
            return View();
        }

        // GET: Writer/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Writer/Register
        [HttpPost]
        public IActionResult Register(Writer writer)
        {
            if (ModelState.IsValid)
            {
                // Writer'ı veritabanına ekleyelim.
                writerManager.TAdd(writer);

                // WriterMail bilgisine göre klasör adını oluştur.
                // Örneğin "writer@mail.com" → "writer_mail_com"
                string folderName = SanitizeFolderName(writer.WriterMail);

                // wwwroot/labourpestcustomer altında writer'a özel klasörü oluştur.
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Kayıt sonrası yönlendirme (örneğin giriş sayfasına)
                return RedirectToAction("Index", "Home");
            }
            return View(writer);
        }

        // Yardımcı metot: Klasör adındaki özel karakterleri temizler.
        private string SanitizeFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return "default_folder";
            return folderName.Replace("@", "_").Replace(".", "_");
        }

        // Örnek Test aksiyonu: Giriş yapan writer'ın bilgilerini ve son 3 blogunu getirir.
        public IActionResult Test()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            // Giriş yapan kullanıcının bilgilerini getiriyoruz.
            Writer writer = writerManager.TGetByID(writerId);

            // Giriş yapan kullanıcının son 3 blogunu getiriyoruz.
            var blogs = blogManager.GetRecentBlogsByWriter(writerId, 3);

            // ViewModel'e dolduruyoruz.
            ProfileViewModel viewModel = new ProfileViewModel
            {
                Writer = writer,
                Blogs = blogs
            };

            return View(viewModel);
        }
    }
}
