using BusinessLayer.Abstract;
using EntityLayer.Concrete; // İçerideki 'Blog' sınıfını buradan okuyacak
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // 1. READ (Listeleme) - GET: api/Blogs
        [HttpGet]
        public IActionResult GetBlogList()
        {
            var values = _blogService.GetAll();
            return Ok(values);
        }

        // 2. READ (Tek Bir Blog Getirme) - GET: api/Blogs/5
        [HttpGet("{id}")]
        public IActionResult GetBlogById(int id)
        {
            try
            {
                var value = _blogService.TGetByID(id);

                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı blog veri tabanında bulunamadı.");
                }

                return Ok(value);
            }
            catch (Exception)
            {
                // Arka plan katmanı (EF Core) bulunamadığı için patlarsa buraya düşecek
                return NotFound($"Kral, {id} numaralı blog veri tabanında mevcut değil (Katman koruması tetiklendi).");
            }
        }

        // 3. CREATE (Yeni Blog Ekleme) - POST: api/Blogs
        [HttpPost]
        public IActionResult CreateBlog([FromBody] Blog blog) // 🎯 Doğrusu 'Blog' yapıldı
        {
            blog.BlogStatus = true;
            blog.BlogDate = DateTime.Now;

            // ✅ EF Core Nesne Takibini engellemek için alt ilişkiler kesiliyor
            blog.BlogCategory = null;
            blog.Writer = null;
            blog.BlogComments = null;
            blog.BlogTags = null;

            _blogService.TAdd(blog);
            return StatusCode(201, "Blog aslanlar gibi eklendi, kral!");
        }

        // 4. UPDATE (Blog Güncelleme) - PUT: api/Blogs
        [HttpPut]
        public IActionResult UpdateBlog([FromBody] Blog blog) // 🎯 Doğrusu 'Blog' yapıldı
        {
            var existingBlog = _blogService.TGetByID(blog.BlogID);
            if (existingBlog == null)
            {
                return NotFound("Güncellenecek blog bulunamadı.");
            }

            // ✅ PUT işleminde de EF Core hata vermesin diye ilişkiler temizleniyor
            blog.BlogCategory = null;
            blog.Writer = null;
            blog.BlogComments = null;
            blog.BlogTags = null;

            _blogService.TUpdate(blog);
            return Ok("Blog başarıyla güncellendi, kral.");
        }

        // 5. DELETE (Blog Silme) - DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBlog(int id)
        {
            var blog = _blogService.TGetByID(id);
            if (blog == null)
            {
                return NotFound("Silinecek blog bulunamadı.");
            }

            _blogService.TDelete(blog);
            return Ok("Blog silindi kral.");
        }
    }
}