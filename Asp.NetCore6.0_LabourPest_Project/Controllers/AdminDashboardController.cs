using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        // Var olan manager nesneleri
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        ImageManager imageManager = new ImageManager(new EfImagesRepository());

        // Yeni: Yorum, Kategori ve Kullanıcı (eğer farklısa) verilerini çekmek için manager örnekleri.
        // Örneğin: 
        CommentManager commentManager = new CommentManager(new EfCommentRepository());
        CategoryManager categoryManager = new CategoryManager(new EfCategoryRepository());
        // Eğer kullanıcı sayısını yazarlardan (Writer) çekiyorsanız, writerManager kullanabilirsiniz.



        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            // Yazar bilgileri
            Writer writer = writerManager.TGetByID(writerId);
            var blogs = blogManager.GetRecentBlogsByWriter(writerId, 3);
            var allImages = imageManager.GetAll() ?? new List<EntityLayer.Concrete.Image>();
            var recentImages = allImages.OrderByDescending(i => i.ImageID)
                                        .Take(6)
                                        .ToList();

            // Dashboard verileri (manager’larınızın GetAll() metotları üzerinden örnek çekim yapılmıştır)
            int blogCount = blogManager.GetAll().Count();
            int commentCount = commentManager.GetAll().Count();
            int imageCount = imageManager.GetAll().Count();
            int categoryCount = categoryManager.GetAll().Count();
            int userCount = writerManager.GetAll().Count();

            ProfileViewModel viewModel = new ProfileViewModel
            {
                Writer = writer,
                Blogs = blogs,
                Images = recentImages,
                BlogCount = blogCount,
                CommentCount = commentCount,
                ImageCount = imageCount,
                CategoryCount = categoryCount,
                UserCount = userCount
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ProfileSettings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            Writer writer = writerManager.TGetByID(writerId);
            return View(writer);
        }

        // POST: /AdminDashboard/ProfileSettings
        [HttpPost]
        public IActionResult ProfileSettings(Writer writer, IFormFile profileImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            Writer currentWriter = writerManager.TGetByID(writerId);

            // Güncellenecek alanları set ediyoruz.
            currentWriter.WriterName = writer.WriterName;
            currentWriter.WriterSurname = writer.WriterSurname;
            currentWriter.WriterMail = writer.WriterMail;
            currentWriter.WriterAbout = writer.WriterAbout;
            currentWriter.WriterPassword = writer.WriterPassword;

            // Profil resmi yüklenmişse, dosyayı kaydedip WriterImage alanını güncelleyelim.
            if (profileImage != null && profileImage.Length > 0)
            {
                // wwwroot klasörü içinde kaydetmek için klasör yolunu oluşturuyoruz.
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "profileImage");

                // Eğer klasör mevcut değilse oluşturuyoruz.
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Dosya adını benzersiz yapmak için Guid kullanıyoruz.
                string fileExtension = Path.GetExtension(profileImage.FileName);
                string newFileName = Guid.NewGuid().ToString() + fileExtension;
                string fullPath = Path.Combine(folderPath, newFileName);

                // Dosyayı kaydediyoruz.
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }

                // WriterImage alanına wwwroot sonrası yol atayabilirsiniz.
                // Örneğin, "/labourpestcustomer/profileImage/yenidosya.jpg" şeklinde.
                currentWriter.WriterImage = "/labourpestcustomer/profileImage/" + newFileName;
            }

            // Güncellemeyi kaydediyoruz.
            writerManager.TUpdate(currentWriter);

            return RedirectToAction("ProfileSettings","AdminDashboard");
        }
    
}
}

