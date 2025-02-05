using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	[AllowAnonymous]
	public class BlogController : Controller
    {
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        BlogCommentManager blogCommentManager = new BlogCommentManager(new EfBlogCommentRepository());
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
		Context _context = new();
		private readonly BlogManager _blogManager;

		public BlogController(BlogManager blogManager)
		{
			_blogManager = blogManager;
		}

        public IActionResult BlogDetails(int id)
        {
            // İlgili blogu veritabanından çekelim.
            var blog = blogManager.TGetByID(id);
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
            int index = DateTime.Now.DayOfYear % quotes.Count;
            string dailyQuote = quotes[index];

            // Günün sözü ViewBag üzerinden view'a gönderiliyor.
            ViewBag.DailyQuote = dailyQuote;

            // (İsterseniz burada ViewBag.id'yi de ayarlayabilirsiniz.)
            ViewBag.id = blog.BlogID; // Bu satırı ekleyebilirsiniz.

            return View(blog);
        }

        // POST: Yorum ekleme
        [HttpPost]
        public IActionResult AddComment(int BlogID, string author, string email, string title, string comment)
        {
            // Giriş yapan kullanıcının (yazarın) bilgilerini çekelim.
            // Burada 'email' parametresi form üzerinden geliyorsa; daha güvenilir olması için User.Identity.Name veya benzeri bir yöntem kullanılabilir.
            var writer = writerManager.GetAll()
                                      .FirstOrDefault(w => w.WriterMail.Equals(email, StringComparison.OrdinalIgnoreCase));
            // Eğer writer bulunamazsa varsayılan bir resim kullanabilirsiniz.
            string imageUrl = writer != null ? writer.WriterImage : "/images/default-user.png";

            // Yeni yorum nesnesi oluşturuluyor.
            BlogComment newComment = new BlogComment
            {
                BlogID = BlogID,
                BlogCommentUserName = author, // Yorum yapan kişinin adı (giriş yapan kullanıcı)
                BlogCommentContent = comment,
                BlogCommentDate = DateTime.Now,
                BlogCommentStatus = true, // Yorum otomatik yayınlansın; moderasyona tabi ise false yapabilirsiniz.
                BlogCommentTitle = string.IsNullOrEmpty(title) ? "Yorum" : title,
 // Kullanıcı form üzerinden başlık girmediyse, varsayılan da atayabilirsiniz.
                BlogImageUrl = imageUrl // Yazarın resim URL'si
            };

            // Yorum ekleniyor.
            blogCommentManager.TAdd(newComment);

            // Yorum ekleme işleminden sonra BlogDetails sayfasına yönlendirme.
            return RedirectToAction("BlogDetails", "Blog", new { id = BlogID });
        }




        public IActionResult Index()
        {
            var values = blogManager.GetBlogListWithBlogCategory();
            return View(values);
        }

    }
}
