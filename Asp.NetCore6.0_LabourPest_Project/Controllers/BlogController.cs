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
