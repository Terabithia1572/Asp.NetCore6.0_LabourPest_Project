using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/BlogCategories şeklinde çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoriesController : ControllerBase
    {
        // İş katmanındaki BlogCategory servis arayüzünü içeriye enjekte ediyoruz
        private readonly IBlogCategoryService _blogCategoryService;

        // Constructor ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public BlogCategoriesController(IBlogCategoryService blogCategoryService)
        {
            _blogCategoryService = blogCategoryService;
        }

        // =====================================================================================
        // 1. READ (Tüm Kategorileri Listeleme) - GET: api/BlogCategories
        // =====================================================================================
        [HttpGet]
        public IActionResult GetBlogCategoryList()
        {
            try
            {
                // İş katmanından tüm blog kategorilerini çekiyoruz
                var values = _blogCategoryService.GetAll();

                // Verileri 200 OK kodu ile birlikte geri dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı hatasında 500 kodu ve hata mesajı döner
                return StatusCode(500, $"Kategoriler listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Kategori Getirme) - GET: api/BlogCategories/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetBlogCategoryById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait kategoriyi iş katmanından sorguluyoruz
                var value = _blogCategoryService.TGetByID(id);

                // Eğer veri tabanında böyle bir kategori yoksa 404 Bulunamadı döner
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı kategori veri tabanında bulunamadı.");
                }

                // Kayıt varsa 200 OK ile veriyi teslim eder
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı kategori getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Kategori Ekleme) - POST: api/BlogCategories
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateBlogCategory([FromBody] BlogCategory blogCategory)
        {
            try
            {
                // Yeni eklenen kategorinin durumunu varsayılan olarak aktif (true) yapıyoruz
                blogCategory.BlogCategoryStatus = true;

                // 🎯 EF CORE TRACKING KORUMASI:
                // Kategoriye bağlı olabilecek blog listesini null yapıyoruz ki EF Core kafa karışıklığı yaşamasın.
                blogCategory.Blogs = null;

                // İş katmanındaki TAdd metodunu tetikleyerek kategoriyi ekliyoruz
                _blogCategoryService.TAdd(blogCategory);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Yeni kategori aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kategori eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Kategori Bilgisini Güncelleme) - PUT: api/BlogCategories
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateBlogCategory([FromBody] BlogCategory blogCategory)
        {
            try
            {
                // 1. Güncellenmek istenen kategorinin gerçekten var olup olmadığını ID ile sorguluyoruz.
                var existingCategory = _blogCategoryService.TGetByID(blogCategory.BlogCategoryID);
                if (existingCategory == null)
                {
                    return NotFound("Güncellenecek kategori kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Veri tabanından gelen ve takip altında (Tracked) olan nesnenin alanlarını, 
                // senin ilk şemadaki property isimlerine göre birebir eşitliyoruz kral:
                existingCategory.BlogCategories = blogCategory.BlogCategories;
                existingCategory.BlogCategoryStatus = blogCategory.BlogCategoryStatus;

                // İlişkili listeyi ezmesin diye null güvencesine alıyoruz
                existingCategory.Blogs = null;

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _blogCategoryService.TUpdate(existingCategory);

                return Ok("Kategori bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kategori güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Kategori Silme) - DELETE: api/BlogCategories/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteBlogCategory(int id)
        {
            try
            {
                // Silinmek istenen kategoriyi ID üzerinden arıyoruz
                var blogCategory = _blogCategoryService.TGetByID(id);
                if (blogCategory == null)
                {
                    return NotFound("Silinecek kategori kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna göndererek siliyoruz
                _blogCategoryService.TDelete(blogCategory);

                return Ok("Kategori veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                // Eğer bu kategoriye bağlı aktif blog yazıları varsa SQL yabancı anahtar (FK) hatası verebilir, onu yakalar:
                return BadRequest($"Kategori silinirken bir hata meydana geldi. Bu kategoriye bağlı blog yazıları olabilir kral: {ex.Message}");
            }
        }
    }
}