using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin,Müşteri")]
    public class AdminDashboardController : Controller
    {
        // Var olan manager nesneleri
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        ImageManager imageManager = new ImageManager(new EfImagesRepository());

        // Yeni: Yorum, Kategori ve Kullanıcı (eğer farklıysa) verilerini çekmek için manager örnekleri.
        CommentManager commentManager = new CommentManager(new EfCommentRepository());
        CategoryManager categoryManager = new CategoryManager(new EfCategoryRepository());

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

            // Dashboard verileri
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

            // Günün sözleri listesi
            var quotes = new List<string>
            {
                "Başarı, sabır ve azimle gelir.",
                "Hayallerine inan, çünk onlar seni geleceğine taşır.",
                "Bugün yaptığın şey yarının temelini oluşturur.",
                "Başarı, asla pes etmeyenlerindir.",
                "Her düşüş bir ders, her kalkış bir zaferdir.",
                "Kendi hikayenin kahramanı ol.",
                "Başarı, hazırlık ve fırsatın kesistiği yerdedir.",
                "Büyük başarılar, küçük ama tutarlı adımlarla gelir.",
                "Yol uzun olabilir, ama her adım seni zirveye taşır.",
                "Düşüncelerine dikkat et, çünk onlar senin geleceğine taşır.",
                "Küçük bir adım bile büyük değişiklikler yaratabilir.",
                "Hata yapmaktan korkma, çünk onlar başarının anahtarlıdır.",
                "Düşüncelerini değiştir, hayatın değişsin.",
                "Her sabah yeni bir başlangıçtır.",
                "En karanlık anların arkasında ışık vardır.",
                "Bir fikrin varsa, ona inan ve peşinden git.",
                "Hayallerini gerçekleştirmek için bugün bir adım at.",
                "Korkuların seni durdurmasına izin verme.",
                "Zorluklar, başarının habercisidir.",
                "Hayatta ne kadar ileri gideceğin, cesaretine bağlıdır.",
                "Pozitif bir zihin, pozitif bir hayat yaratır.",
                "Hayatta en önemli şey, kendi şansını yaratmaktır.",
                "Her yeni gün, yeni bir fırsat getirir.",
                "Gülümsemek, en güzel enerjidir.",
                "Hayat, sen ne düşünüyorsan onu sana geri verir.",
                "Kendine inan; bu, başarının yarısıdır.",
                "Zorluklar seni daha güçlü yapar.",
                "Her düşüş, kalkmak için bir fırsattır.",
                "Küçük mutluluklar, büyük mutlulukları getirir."
            };

            // Günün sözünü hesaplamak için: 
            // DateTime.Now.DayOfYear yılın kaçıncı günü olduğunu verir.
            int index = DateTime.Now.DayOfYear % quotes.Count;
            ViewBag.DailyQuote = quotes[index];

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

            // Eğer giriş yapan kullanıcı Müşteri ise WriterAbout alanını zorunlu "Müşteri" olarak güncelle
            if (User.IsInRole("Müşteri"))
            {
                currentWriter.WriterAbout = "Müşteri";
            }
            else
            {
                currentWriter.WriterAbout = writer.WriterAbout;
            }

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

                // WriterImage alanına wwwroot sonrası yol atıyoruz.
                currentWriter.WriterImage = "/labourpestcustomer/profileImage/" + newFileName;
            }

            // Güncellemeyi kaydediyoruz.
            writerManager.TUpdate(currentWriter);

            return RedirectToAction("ProfileSettings", "AdminDashboard");
        }
    }
}
