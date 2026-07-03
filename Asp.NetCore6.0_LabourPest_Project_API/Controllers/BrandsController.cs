using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Brands şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        // İş katmanındaki Brands servis arayüzünü içeriye enjekte ediyoruz
        private readonly IBrandsService _brandsService;

        // Constructor ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public BrandsController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        // =====================================================================================
        // 1. READ (Tüm Markaları/Referansları Listeleme) - GET: api/Brands
        // =====================================================================================
        [HttpGet]
        public IActionResult GetBrandList()
        {
            try
            {
                // İş katmanından tüm marka verilerini çekiyoruz
                var values = _brandsService.GetAll();

                // Verileri 200 OK kodu ile birlikte istemciye (Flutter) dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Markalar listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Marka Getirme) - GET: api/Brands/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id)
        {
            try
            {
                var value = _brandsService.TGetByID(id);

                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı marka bulunamadı.");
                }

                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı marka getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Marka Ekleme) - POST: api/Brands
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateBrand([FromBody] Brands brand)
        {
            try
            {
                // 🎯 DOĞRULAMA: Görseldeki BrandsStatus alanını yeni kayıtta varsayılan olarak aktif (true) yapıyoruz
                brand.BrandsStatus = true;

                _brandsService.TAdd(brand);
                return StatusCode(201, "Yeni marka logosu aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Marka eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Marka Güncelleme) - PUT: api/Brands
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateBrand([FromBody] Brands brand)
        {
            try
            {
                // 1. Güncellenmek istenen markayı veri tabanından mevcut ID ile çekiyoruz
                // 🎯 DOĞRULAMA: Birincil anahtar alanı 'BrandsID' olarak eşitlendi.
                var existingBrand = _brandsService.TGetByID(brand.BrandsID);
                if (existingBrand == null)
                {
                    return NotFound("Güncellenecek marka kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Sütun isimlerini senin gönderdiğin şemaya göre birebir eşitliyoruz kral:
                existingBrand.BrandsImage = brand.BrandsImage;
                existingBrand.BrandsStatus = brand.BrandsStatus;

                // Zaten takip altında olan nesneyi iş katmanına gönderiyoruz
                _brandsService.TUpdate(existingBrand);

                return Ok("Marka bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Marka güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Marka Silme) - DELETE: api/Brands/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(int id)
        {
            try
            {
                var brand = _brandsService.TGetByID(id);
                if (brand == null)
                {
                    return NotFound("Silinecek marka kaydı bulunamadı, kral.");
                }

                _brandsService.TDelete(brand);
                return Ok("Marka veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Marka silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}