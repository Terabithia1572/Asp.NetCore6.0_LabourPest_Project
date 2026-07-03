using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Products şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // İş katmanındaki Products servis arayüzünü içeriye enjekte ediyoruz
        private readonly IProductService _productsService;

        // Constructor (Yapıcı Metot) ile bağımlılığı içeri alıyoruz
        public ProductsController(IProductService productsService)
        {
            _productsService = productsService;
        }

        // =====================================================================================
        // 1. READ (Tüm Ürünleri Listeleme) - GET: api/Products
        // =====================================================================================
        [HttpGet]
        public IActionResult GetProductList()
        {
            try
            {
                // İş katmanından tüm ürün verilerini çekiyoruz
                var values = _productsService.GetListWithProductCategory();

                // Verileri 200 OK kodu ile birlikte istemciye (Flutter) dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ürünler listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Ürün Getirme) - GET: api/Products/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var value = _productsService.TGetByID(id);
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı ürün veri tabanında bulunamadı.");
                }
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı ürün getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Ürün Ekleme - İlişkili Kategori ID ile) - POST: api/Products
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            try
            {
                // 🎯 DOĞRULAMA: Görseldeki gerçek kolon adına göre durum bilgisini aktif (true) yapıyoruz
                product.ProductStatus = true;

                // Swagger veya Flutter'dan gelen JSON verisinde "Category": {} nesnesi dolu gelirse, 
                // EF Core sıfırdan kategori eklemeye çalışmasın diye ilişkisel nesneyi null yapıyoruz.
                product.Category = null;

                _productsService.TAdd(product);
                return StatusCode(201, "Yeni ürün, kategorisiyle ilişkili şekilde aslanlar gibi eklendi kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ürün eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Ürün Bilgisini ve Kategorisini Güncelleme) - PUT: api/Products
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            try
            {
                // 1. Güncellenmek istenen asıl ürünü mevcut ID ile veri tabanından çekip takibe (Tracking) alıyoruz.
                // 🎯 DOĞRULAMA: Görseldeki gibi birincil anahtar alanı 'ProductID' olarak eşitlendi.
                var existingProduct = _productsService.TGetByID(product.ProductID);
                if (existingProduct == null)
                {
                    return NotFound("Güncellenecek ürün kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING VE İLİŞKİ ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // 🎯 DOĞRULAMA: Sütun isimlerini son attığın görselle (%100) birebir eşitledik kral:
                existingProduct.ProductName = product.ProductName;
                existingProduct.ProductDescription = product.ProductDescription;
                existingProduct.ProductImageURL = product.ProductImageURL;
                existingProduct.ProductStatus = product.ProductStatus;

                // Ürünün bağlı olduğu kategoriyi değiştirmek için ham Foreign Key hücresini eşitliyoruz
                existingProduct.CategoryID = product.CategoryID;

                // İlişkili navigation nesnesini ezmesin veya çakışma yaratmasın diye null güvencesine alıyoruz
                existingProduct.Category = null;

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _productsService.TUpdate(existingProduct);

                return Ok("Ürün ve kategori ilişkisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ürün güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Ürün Silme) - DELETE: api/Products/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _productsService.TGetByID(id);
                if (product == null)
                {
                    return NotFound("Silinecek ürün kaydı bulunamadı, kral.");
                }

                _productsService.TDelete(product);
                return Ok("Ürün veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ürün silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}