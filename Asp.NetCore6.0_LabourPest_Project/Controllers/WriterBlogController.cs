using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class WriterBlogController : Controller
    {
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MyRecentBlogs()
        {
            // blogManager.GetRecentBlogsByWriter(writerId, count) şeklinde bir metot düşünelim.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            // Son 3 blog gönderisini getiriyoruz
            var blogs = blogManager.GetRecentBlogsByWriter(writerId, 3);
            return View(blogs);
        }
        public IActionResult BlogList()
        {
            var values = blogManager.GetBlogListWithBlogCategory();
            ViewBag.blogSayisi = blogManager.GetBlogListWithBlogCategory().Count();
            return View(values);
        }
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
        [HttpPost]
        public IActionResult AddBlog(Blog blog)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            blog.BlogStatus = true;
            blog.BlogDate = DateTime.Now;
            blog.WriterID = writerId;
            blogManager.TAdd(blog);

            return RedirectToAction("BlogListByWriter", "WriterBlog");
        }

        public IActionResult DeleteBlog(int id)
        {
            var values = blogManager.TGetByID(id);
            blogManager.TDelete(values);
            return RedirectToAction("BlogListByWriter", "WriterBlog");
        }

        public  IActionResult BlogListByWriter()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);
            ViewBag.writerId = writerId;
            var values = blogManager.GetListWithCategoryByWriter(writerId);
            return View(values);

        }

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

