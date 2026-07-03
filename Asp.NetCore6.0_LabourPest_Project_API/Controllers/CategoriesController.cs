using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Categories şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // İş katmanındaki Categories servis arayüzünü içeriye enjekte ediyoruz
        private readonly ICategoryService _categoriesService;

        // Constructor (Yapıcı Metot) ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public CategoriesController(ICategoryService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        // =====================================================================================
        // 1. READ (Tüm Ürün Kategorilerini Listeleme) - GET: api/Categories
        // =====================================================================================
        [HttpGet]
        public IActionResult GetCategoryList()
        {
            try
            {
                // İş katmanından tüm ürün kategorilerini çekiyoruz
                var values = _categoriesService.GetAll();

                // Verileri 200 OK kodu ile birlikte istemciye (Flutter) dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı hatasında 500 kodu ve hata mesajı döner
                return StatusCode(500, $"Kategoriler listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Kategori Getirme) - GET: api/Categories/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait kategoriyi iş katmanından sorguluyoruz
                var value = _categoriesService.TGetByID(id);

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
        // 3. CREATE (Yeni Ürün Kategorisi Ekleme) - POST: api/Categories
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            try
            {
                // 🎯 DOĞRULAMA: İstek şemana uygun olarak yeni kategorinin durumunu aktif (true) yapıyoruz
                category.CategoryStatus = true;

                // 🎯 EF CORE TRACKING KORUMASI:
                // Kategoriye bağlı olabilecek ürün listesini (Products) null yapıyoruz ki 
                // EF Core deserialize işleminde döngüsel tip hataları fırlatmasın.
                category.Products = null;

                // İş katmanındaki TAdd metodunu tetikleyerek yeni kategoriyi ekliyoruz
                _categoriesService.TAdd(category);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Yeni ürün kategorisi aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kategori eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Kategori Bilgisini Güncelleme) - PUT: api/Categories
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateCategory([FromBody] Category category)
        {
            try
            {
                // 1. Güncellenmek istenen kaydın gerçekten var olup olmadığını ID ile sorguluyoruz.
                // 🎯 DOĞRULAMA: İstek şemana uygun olarak birincil anahtarı 'CategoryID' kabul ettik.
                var existingCategory = _categoriesService.TGetByID(category.CategoryID);
                if (existingCategory == null)
                {
                    return NotFound("Güncellenecek kategori kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Veri tabanından gelen ve takip altında (Tracked) olan nesnenin alanlarını eşitleyerek güncelliyoruz.
                // Sütun isimlerini senin son Swagger şemana göre birebir eşitledik kral:
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.CategoryDescription = category.CategoryDescription;
                existingCategory.CategoryImages = category.CategoryImages;
                existingCategory.CategoryStatus = category.CategoryStatus;

                // İlişkili listeyi ezmesin veya çakışma yaratmasın diye null güvencesine alıyoruz
                existingCategory.Products = null;

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _categoriesService.TUpdate(existingCategory);

                return Ok("Kategori bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kategori güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Kategori Silme) - DELETE: api/Categories/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                // Silinmek istenen kaydı ID üzerinden arıyoruz
                var category = _categoriesService.TGetByID(id);
                if (category == null)
                {
                    return NotFound("Silinecek kategori kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna göndererek siliyoruz
                _categoriesService.TDelete(category);

                return Ok("Kategori veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                // Eğer bu kategoriye bağlı ürünler varsa SQL Foreign Key hatası verir, onu yakalıyoruz:
                return BadRequest($"Kategori silinirken hata meydana geldi. Bu kategoriye bağlı aktif ürünler olabilir kral: {ex.Message}");
            }
        }
    }
}