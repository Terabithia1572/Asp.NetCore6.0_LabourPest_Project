using Asp.NetCore6._0_LabourPest_Project.Hubs;
using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class BlogController : Controller
    {
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        BlogCommentManager blogCommentManager = new BlogCommentManager(new EfBlogCommentRepository());
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        NotificationManager notificationManager = new NotificationManager(new EfNotificationRepository());
        Context _context = new();
        private readonly BlogManager _blogManager;
        private readonly IHubContext<NotificationHub> _hubContext;


        public BlogController(BlogManager blogManager, IHubContext<NotificationHub> hubContext)
        {
            _blogManager = blogManager;
            _hubContext = hubContext;

        }

        public IActionResult BlogDetails(string slug)
        {
            var blog = _context.Blogs
     .Include(b => b.Writer)
     .Include(b => b.BlogCategory)
     .FirstOrDefault(b => b.SlugUrl == slug);

            if (blog == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                string userEmail = User.Identity.Name;
                var writer = writerManager.GetAll()
                    .FirstOrDefault(w => w.WriterMail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                if (writer != null)
                {
                    ViewBag.CurrentWriterName = writer.WriterName + " " + writer.WriterSurname;
                    ViewBag.CurrentWriterMail = writer.WriterMail;
                }
            }

            // Günün Sözü: Söz listesini tanımlıyoruz.
            var quotes = new List<string>
    {
        "Başarı, sabır ve azimle gelir.",
        "Hayallerine inan, çünkü onlar seni geleceğine taşır.",
        "Bugün yaptığın şey yarının temelini oluşturur.",
        "Başarı, asla pes etmeyenlerindir.",
        "Her düşüş bir ders, her kalkış bir zaferdir.",
        "Kendi hikayenin kahramanı ol.",
        "Başarı, hazırlık ve fırsatın kesiştiği yerdedir.",
        "Büyük başarılar, küçük ama tutarlı adımlarla gelir.",
        "Yol uzun olabilir, ama her adım seni zirveye taşır.",
        "Düşüncelerine dikkat et, çünkü onlar senin geleceğindir.",
        "Küçük bir adım bile büyük değişiklikler yaratabilir.",
        "Hata yapmaktan korkma, çünkü onlar başarının anahtarıdır.",
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
        "Küçük mutluluklar, büyük mutlulukları getirir.",
        "Hayatın her anı bir hediyedir."
    };

            // Günün sözünü belirlemek için indeksi hesaplıyoruz.
            // Örneğin: yılın kaçıncı günü olduğunu kullanıyoruz.
            int index = DateTime.Now.DayOfYear % quotes.Count; // Bu şekilde her gün farklı bir söz gösterilecektir.
            ViewBag.DailyQuote = quotes[index]; // Günün sözü
            ViewBag.id = blog.BlogID; // Blog ID'si

            return View(blog); // Blog detaylarını döndürüyoruz.
        }

        // POST: Yorum ekleme
        [HttpPost]
        public async Task<IActionResult> AddComment(int BlogID, string author, string email, string title, string comment, int? ParentCommentID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            var writer = writerManager.TGetByID(writerId);

            string imageUrl = writer?.WriterImage ?? "/images/default-user.png";

            BlogComment newComment = new BlogComment
            {
                BlogID = BlogID,
                BlogCommentUserName = writer.WriterName + " " + writer.WriterSurname, // 👈 NULL gelmesin
                BlogCommentContent = comment,
                BlogCommentDate = DateTime.Now,
                BlogCommentStatus = true,
                BlogCommentTitle = string.IsNullOrEmpty(title) ? "Yorum" : title,
                BlogImageUrl = imageUrl,
                WriterID = writer.WriterID,
                ParentCommentID = ParentCommentID
            };

            blogCommentManager.TAdd(newComment);

            // Bildirim
            var blog = blogManager.TGetByID(BlogID);
            var notification = new Notification
            {
                NotificationType = "Comment",
                NotificationMessage = $"{writer.WriterName} adlı kullanıcı blogunuza yorum yaptı.",
                NotificationDate = DateTime.Now,
                NotificationStatus = false,
                WriterID = blog.WriterID,
                SenderWriterID = writer.WriterID,
                NotificationUrl = "/Blog/BlogDetails?slug=" + blog.SlugUrl
            };

            notificationManager.TAdd(notification);

            await _hubContext.Clients
                .User(blog.WriterID.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    message = notification.NotificationMessage,
                    url = notification.NotificationUrl,
                    time = notification.NotificationDate.ToString("g")
                });

            return RedirectToAction("BlogDetails", "Blog", new { slug = blog.SlugUrl });
        }









        public IActionResult Index()
        {
            var values = blogManager.GetBlogListWithBlogCategory();
            return View(values);
        }

        // BlogController.cs
        [HttpGet]
        public IActionResult EditComment(int id)
        {
            var comment = _context.BlogsComments
                .Include(x => x.Blog)
                .FirstOrDefault(x => x.BlogCommentID == id);

            if (comment == null)
                return NotFound();

            return View(comment);
        }

        [HttpPost]
        public IActionResult EditComment(BlogComment updatedComment)
        {
            var comment = _context.BlogsComments
                .FirstOrDefault(x => x.BlogCommentID == updatedComment.BlogCommentID);

            if (comment == null)
                return NotFound();

            comment.BlogCommentTitle = updatedComment.BlogCommentTitle;
            comment.BlogCommentContent = updatedComment.BlogCommentContent;
            comment.BlogCommentDate = DateTime.Now;

            _context.SaveChanges();

            // blog detay sayfasına yönlendir
            var blog = _context.Blogs.FirstOrDefault(x => x.BlogID == comment.BlogID);
            if (blog != null)
                return RedirectToAction("BlogDetails", "Blog", new { slug = blog.SlugUrl });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateCommentAjax(int id, string title, string content)
        {
            var comment = _context.BlogsComments.FirstOrDefault(x => x.BlogCommentID == id);
            if (comment == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı." });
            }

            comment.BlogCommentTitle = title;
            comment.BlogCommentContent = content;
            comment.BlogCommentDate = DateTime.Now;
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                updatedTitle = comment.BlogCommentTitle,
                updatedContent = comment.BlogCommentContent,
                updatedDate = comment.BlogCommentDate.ToString("dd MMM yyyy HH:mm")
            });
        }




        private string CreateSlug(string phrase)
        {
            var dict = new Dictionary<string, string>()
    {
        { "ş", "s" }, { "Ş", "s" },
        { "ı", "i" }, { "İ", "i" },
        { "ğ", "g" }, { "Ğ", "g" },
        { "ü", "u" }, { "Ü", "u" },
        { "ö", "o" }, { "Ö", "o" },
        { "ç", "c" }, { "Ç", "c" }
    };

            foreach (var entry in dict)
            {
                phrase = phrase.Replace(entry.Key, entry.Value);
            }

            phrase = phrase.ToLowerInvariant();
            phrase = Regex.Replace(phrase, @"[^a-z0-9\s-]", "");  // Geçersiz karakterleri sil
            phrase = Regex.Replace(phrase, @"\s+", " ").Trim();  // Fazla boşlukları temizle
            phrase = Regex.Replace(phrase, @"\s", "-");          // Boşlukları tire ile değiştir
            phrase = Regex.Replace(phrase, @"-+", "-");          // Çift tireleri teke indir

            return phrase;
        }
    }
}
