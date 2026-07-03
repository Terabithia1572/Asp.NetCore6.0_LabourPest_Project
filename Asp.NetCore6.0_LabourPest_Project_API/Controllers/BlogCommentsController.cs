using BusinessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 🎯 Diğer katmanlardan bağımsız sorgular için şart kral

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCommentsController : ControllerBase
    {
        private readonly IBlogCommentService _blogCommentService;

        public BlogCommentsController(IBlogCommentService blogCommentService)
        {
            _blogCommentService = blogCommentService;
        }

        // =====================================================================================
        // 1. READ (Tüm Blog Yorumlarını Listeleme) - GET: api/BlogComments
        // =====================================================================================
        [HttpGet]
        public IActionResult GetBlogCommentList()
        {
            try
            {
                // 🎯 REPAİR OPERASYONU BURASI KRAL:
                // Katmanlardaki otomatik üretilen hatalı SQL sorgularını bypass ediyoruz.
                // Doğrudan temiz Context üzerinden, sadece senin attığın şemadaki gerçek sütunları seçiyoruz (Select).
                using var context = new Context(); // DbContext adın neyse (Context)

                var values = context.BlogsComments
                                    .Select(c => new
                                    {
                                        c.BlogCommentID,
                                        c.BlogCommentUserName,
                                        c.BlogCommentTitle,
                                        c.BlogCommentContent,
                                        c.BlogImageUrl,
                                        c.BlogCommentDate,
                                        c.BlogCommentStatus,
                                        c.BlogID,
                                        c.ParentCommentID
                                    })
                                    .ToList();

                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Yorumlar listelenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Yorum Getirme) - GET: api/BlogComments/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetBlogCommentById(int id)
        {
            try
            {
                using var context = new Context();

                var value = context.BlogsComments
                                   .Where(c => c.BlogCommentID == id)
                                   .Select(c => new
                                   {
                                       c.BlogCommentID,
                                       c.BlogCommentUserName,
                                       c.BlogCommentTitle,
                                       c.BlogCommentContent,
                                       c.BlogImageUrl,
                                       c.BlogCommentDate,
                                       c.BlogCommentStatus,
                                       c.BlogID,
                                       c.ParentCommentID
                                   })
                                   .FirstOrDefault();

                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı blog yorumu bulunamadı.");
                }
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Blog Yorumu / Cevap Yorumu Ekleme) - POST: api/BlogComments
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateBlogComment([FromBody] BlogComment blogComment)
        {
            try
            {
                blogComment.BlogCommentDate = DateTime.Now;
                blogComment.BlogCommentStatus = true;

                // EF Core'un arkada uydurma Writer veya Blog tablosu aramasını engellemek için 
                // nesne hiyerarşilerini tamamen null çekip sadece ID'leri eziyoruz.
                blogComment.Blog = null;

                _blogCommentService.TAdd(blogComment);
                return StatusCode(201, "Blog yorumu aslanlar gibi eklendi kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yorum eklenirken hata: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Blog Yorumu Güncelleme) - PUT: api/BlogComments
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateBlogComment([FromBody] BlogComment blogComment)
        {
            try
            {
                var existing = _blogCommentService.TGetByID(blogComment.BlogCommentID);
                if (existing == null)
                {
                    return NotFound("Güncellenecek yorum bulunamadı kral.");
                }

                existing.BlogCommentUserName = blogComment.BlogCommentUserName;
                existing.BlogCommentTitle = blogComment.BlogCommentTitle;
                existing.BlogCommentContent = blogComment.BlogCommentContent;
                existing.BlogImageUrl = blogComment.BlogImageUrl;
                existing.BlogCommentStatus = blogComment.BlogCommentStatus;
                existing.BlogID = blogComment.BlogID;
                existing.ParentCommentID = blogComment.ParentCommentID;

                existing.Blog = null;

                _blogCommentService.TUpdate(existing);
                return Ok("Blog yorumu başarıyla güncellendi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yorum güncellenirken hata: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Blog Yorumu Silme) - DELETE: api/BlogComments/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteBlogComment(int id)
        {
            try
            {
                var comment = _blogCommentService.TGetByID(id);
                if (comment == null)
                {
                    return NotFound("Silinecek yorum bulunamadı.");
                }

                _blogCommentService.TDelete(comment);
                return Ok("Blog yorumu başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yorum silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}