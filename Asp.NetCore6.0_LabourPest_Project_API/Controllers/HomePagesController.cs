using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePagesController : ControllerBase
    {
        private readonly IHomePageService _homePageService;

        public HomePagesController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }

        // =====================================================================================
        // 1. READ (Ana Sayfa Bilgilerini Listeleme) - GET: api/HomePages
        // =====================================================================================
        [HttpGet]
        public IActionResult GetHomePageList()
        {
            try
            {
                var values = _homePageService.GetAll();
                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ana sayfa bilgileri listelenirken hata oluştu: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Kayıt Getirme) - GET: api/HomePages/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetHomePageById(int id)
        {
            try
            {
                var value = _homePageService.TGetByID(id);
                if (value == null) return NotFound($"Kral, {id} numaralı veri bulunamadı.");
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound("Katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Ana Sayfa İçeriği Ekleme) - POST: api/HomePages
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateHomePage([FromBody] HomePage homePage)
        {
            try
            {
                // 🎯 DOĞRULAMA: Sütun yapındaki HomePageStatus alanını varsayılan olarak aktif yapıyoruz
                homePage.HomePageStatus = true;

                _homePageService.TAdd(homePage);
                return StatusCode(201, "Ana sayfa verisi aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ekleme hatası: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Ana Sayfa İçeriğini Güncelleme) - PUT: api/HomePages
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateHomePage([FromBody] HomePage homePage)
        {
            try
            {
                // 1. Güncellenecek kaydı ID ile veri tabanından çekip takibe alıyoruz
                // 🎯 DOĞRULAMA: Birincil anahtar alanı 'HomePageID' olarak eşitlendi.
                var existing = _homePageService.TGetByID(homePage.HomePageID);
                if (existing == null) return NotFound("Güncellenecek kayıt bulunamadı kral.");

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Sütun isimlerini senin gönderdiğin gerçek şemaya göre birebir eşitliyoruz kral:
                existing.HomePageDescription1 = homePage.HomePageDescription1;
                existing.HomePageDescription2 = homePage.HomePageDescription2;
                existing.HomePageSubDescription = homePage.HomePageSubDescription;
                existing.HomePageImage = homePage.HomePageImage;
                existing.HomePageStatus = homePage.HomePageStatus;

                _homePageService.TUpdate(existing);
                return Ok("Ana sayfa bilgileri başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Güncelleme hatası: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Ana Sayfa İçeriğini Silme) - DELETE: api/HomePages/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteHomePage(int id)
        {
            try
            {
                var homePage = _homePageService.TGetByID(id);
                if (homePage == null) return NotFound("Kayıt bulunamadı, kral.");
                _homePageService.TDelete(homePage);
                return Ok("Ana sayfa verisi başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Silme hatası: {ex.Message}");
            }
        }
    }
}