using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
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
        Context _context = new Context();
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
        [HttpGet]
        [Authorize(Roles = "Admin,Müşteri")]
        public IActionResult AddBlog()
        {
            List<SelectListItem> categoryValues = blogCategoryManager.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.BlogCategories,
                    Value = x.BlogCategoryID.ToString()
                }).ToList();
            ViewBag.cv = categoryValues;

            // Etiketleri çek
            ViewBag.Tags = _context.Tags.Select(t => new SelectListItem
            {
                Text = t.TagName,
                Value = t.TagID.ToString()
            }).ToList();

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Müşteri")]
        public IActionResult AddBlog(Blog blog)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            blog.BlogStatus = true;
            blog.BlogDate = DateTime.Now;
            blog.WriterID = writerId;
            blog.SlugUrl = CreateUniqueSlug(blog.BlogTitle);

            // Önce blog'u kaydet
            blogManager.TAdd(blog);

            // Kaydedilen blogu slug üzerinden tekrar çek
            var savedBlog = _context.Blogs.FirstOrDefault(b => b.SlugUrl == blog.SlugUrl);

            // Seçili checkbox etiketlerini al
            var selectedTagIds = Request.Form["SelectedTags"].ToList();

            // Etiketleri ilişkilendir
            foreach (var tagId in selectedTagIds)
            {
                if (int.TryParse(tagId, out int parsedTagId))
                {
                    _context.BlogTags.Add(new BlogTag
                    {
                        BlogID = savedBlog.BlogID,
                        TagID = parsedTagId
                    });
                }
            }

            _context.SaveChanges();

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
            var blog = blogManager.TGetByID(id);
            if (blog == null) return NotFound();

            // Etiket ilişkilerini sil
            var blogTags = _context.BlogTags.Where(bt => bt.BlogID == id).ToList();
            _context.BlogTags.RemoveRange(blogTags);
            _context.SaveChanges();

            // Blogu sil
            blogManager.TDelete(blog);
            return RedirectToAction("BlogList","AdminBlog");
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
        [HttpGet]
        public IActionResult UpdateBlog(int id)
        {
            // blogu id'ye göre bul
            var blog = blogManager.TGetByID(id);
            if (blog == null) return NotFound();

            // kategori dropdown'u tekrar yolla
            ViewBag.cv = blogCategoryManager.GetAll()
                          .Select(x => new SelectListItem
                          {
                              Text = x.BlogCategories,
                              Value = x.BlogCategoryID.ToString()
                          }).ToList();
            ViewBag.Tags = _context.Tags.Select(t => new SelectListItem
            {
                Text = t.TagName,
                Value = t.TagID.ToString()
            }).ToList();

            ViewBag.SelectedTagIds = _context.BlogTags
                .Where(bt => bt.BlogID == id)
                .Select(bt => bt.TagID)
                .ToList();


            return View(blog); // UpdateBlog.cshtml
        }

        [HttpPost]
        public IActionResult UpdateBlog(Blog blog)
        {
            // Güncellenecek blogu veritabanından al
            var existingBlog = _context.Blogs.FirstOrDefault(b => b.BlogID == blog.BlogID);
            if (existingBlog == null)
                return NotFound();

            // Güncelleme işlemleri
            existingBlog.BlogTitle = blog.BlogTitle;
            existingBlog.BlogContent = blog.BlogContent;
            existingBlog.BlogImage = blog.BlogImage;
            existingBlog.BlogCategoryID = blog.BlogCategoryID;
            existingBlog.BlogDate = DateTime.Now;
            existingBlog.SlugUrl = CreateUniqueSlug(blog.BlogTitle);

            // Var olan etiketleri sil
            var oldTags = _context.BlogTags.Where(bt => bt.BlogID == blog.BlogID);
            _context.BlogTags.RemoveRange(oldTags);

            // Yeni seçilen etiketleri ekle
            var selectedTagIds = Request.Form["SelectedTags"].ToList();
            foreach (var tagId in selectedTagIds)
            {
                if (int.TryParse(tagId, out int parsedTagId))
                {
                    _context.BlogTags.Add(new BlogTag
                    {
                        BlogID = blog.BlogID,
                        TagID = parsedTagId
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("BlogList", "AdminBlog");
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
