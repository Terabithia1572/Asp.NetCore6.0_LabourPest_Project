using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    
    public class AdminBlogController : Controller
    {
        BlogManager blogManager= new BlogManager(new EfBlogRepository());
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult BlogList()
        {
            var values = blogManager.GetBlogListWithBlogCategory();
            ViewBag.blogSayisi = blogManager.GetBlogListWithBlogCategory().Count();
            return View(values);
        }
        [Authorize(Roles = "Admin,Müşteri")]
        [HttpGet]
        public IActionResult AddBlog()
        {
            List<SelectListItem> categoryValues = (from x in blogCategoryManager.GetAll()
                                                   select new SelectListItem
                                                   {
                                                       Text = x.BlogCategories,
                                                       Value = x.BlogCategoryID.ToString()
                                                   }).ToList();
            ViewBag.cv = categoryValues;
            return View();
        }
        [Authorize(Roles = "Admin,Müşteri")]
        [HttpPost]
        public IActionResult AddBlog(Blog blog)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            blog.BlogStatus = true;
            blog.BlogDate = DateTime.Now;
            blog.WriterID = writerId;
            blog.SlugUrl = CreateUniqueSlug(blog.BlogTitle); // benzersiz slug üret
            blogManager.TAdd(blog);

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("BlogList", "AdminBlog");
            }
            else
            {
                return RedirectToAction("Deneme", "Home");
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBlog(int id)
        {
            var values = blogManager.TGetByID(id);
            blogManager.TDelete(values);
            return RedirectToAction("BlogList", "AdminBlog");
        }
        private string CreateSlug(string phrase)
        {
            var dict = new Dictionary<string, string>
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
        private string CreateUniqueSlug(string title)
        {
            // Temel slug'ı üret
            string slug = CreateSlug(title);
            string baseSlug = slug;
            int count = 1;

            // Slug benzersiz mi kontrol et
            while (blogManager.GetAll().Any(b => b.SlugUrl == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }




        [Authorize(Roles = "Admin,Müşteri")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "blogImage");
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
                string relativePath = $"/labourpestcustomer/blogImage/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }

    }
}
