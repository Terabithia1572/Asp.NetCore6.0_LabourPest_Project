using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API'nin dış dünyadan çağrılacağı adresi ekiyoruz: api/WhoWeUs
    [Route("api/[controller]")]
    [ApiController]
    public class WhoWeUsController : ControllerBase
    {
        // İş katmanındaki arayüzümüzü (Interface) içeriye enjekte etmek için tanımlıyoruz
        private readonly IWhoWeUsService _whoWeUsService;

        // Constructor (Yapıcı Metot) vasıtasıyla bağımlılığı içeri alıyoruz
        public WhoWeUsController(IWhoWeUsService whoWeUsService)
        {
            _whoWeUsService = whoWeUsService;
        }

        // =====================================================================================
        // 1. READ (Hakkımızda Bilgilerini Listeleme) - GET: api/WhoWeUs
        // =====================================================================================
        [HttpGet]
        public IActionResult GetWhoWeUsList()
        {
            try
            {
                // İş katmanı üzerinden hakkımızda verilerini çekiyoruz
                var values = _whoWeUsService.GetAll();

                // Başarılıysa verileri 200 OK kodu ile geri döndürüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Sistemde beklenmeyen bir hata çıkarsa yakalayıp 500 koduyla basıyoruz
                return StatusCode(500, $"Hakkımızda bilgileri listelenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Kayıt Getirme) - GET: api/WhoWeUs/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetWhoWeUsById(int id)
        {
            try
            {
                // Gelen ID numarasına göre veri tabanında tekil arama yapıyoruz
                var value = _whoWeUsService.TGetByID(id);

                // Eğer böyle bir kayıt bulunamadıysa istemciye 404 Bulunamadı dönüyoruz
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı kurumsal bilgi veri tabanında bulunamadı.");
                }

                // Kayıt mevcutsa veriyi 200 OK ile teslim ediyoruz
                return Ok(value);
            }
            catch (Exception)
            {
                // Katmanlardaki olası null durumlarına karşı try-catch koruması
                return NotFound($"Kral, {id} numaralı kurumsal bilgi getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Hakkımızda/Kurumsal Bilgi Ekleme) - POST: api/WhoWeUs
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateWhoWeUs([FromBody] WhoWeUs whoWeUs)
        {
            try
            {
                // Görseldeki WhoWeUsStatus alanını yeni kayıt esnasında varsayılan olarak aktif (true) yapıyoruz
                whoWeUs.WhoWeUsStatus = true;

                // İş katmanındaki Add (TAdd) metoduna nesnemizi gönderiyoruz
                _whoWeUsService.TAdd(whoWeUs);

                // Başarılı bir şekilde eklenince 201 Created durum kodu ve mesajımızı veriyoruz
                return StatusCode(201, "Kurumsal bilgi aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                // Validasyon veya SQL kısıtlamasına takılırsa hata fırlatır
                return BadRequest($"Kurumsal bilgi eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Mevcut Hakkımızda Bilgisini Güncelleme) - PUT: api/WhoWeUs
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateWhoWeUs([FromBody] WhoWeUs whoWeUs)
        {
            try
            {
                // 1. Güncellenmek istenen asıl kaydı veri tabanından mevcut ID ile çekiyoruz.
                var existingWhoWeUs = _whoWeUsService.TGetByID(whoWeUs.WhoWeUsID);
                if (existingWhoWeUs == null)
                {
                    return NotFound("Güncellenecek kurumsal bilgi kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Veri tabanından gelen ve takip altında (Tracked) olan nesnenin kolonlarını, 
                // gönderdiğin görseldeki ({9FBD21EF-123D-46FC-9346-AB116FB27A8A}.png) sütun isimleriyle birebir eşliyoruz:
                existingWhoWeUs.WhoWeUsTitle1 = whoWeUs.WhoWeUsTitle1;
                existingWhoWeUs.WhoWeUsTitle2 = whoWeUs.WhoWeUsTitle2;
                existingWhoWeUs.WhoWeUsDescription1 = whoWeUs.WhoWeUsDescription1;
                existingWhoWeUs.WhoWeUsDescription2 = whoWeUs.WhoWeUsDescription2;
                existingWhoWeUs.WhoWeUsImageURL = whoWeUs.WhoWeUsImageURL;
                existingWhoWeUs.WhoWeUsClass = whoWeUs.WhoWeUsClass;
                existingWhoWeUs.WhoWeUsStatus = whoWeUs.WhoWeUsStatus;

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _whoWeUsService.TUpdate(existingWhoWeUs);

                // İşlem başarılıysa 200 OK yanıtını basıyoruz
                return Ok("Kurumsal bilgiler başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kurumsal bilgi güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Kurumsal Bilgi Silme) - DELETE: api/WhoWeUs/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteWhoWeUs(int id)
        {
            try
            {
                // Silinmek istenen kaydı önce ID parametresi üzerinden kontrol ediyoruz
                var whoWeUs = _whoWeUsService.TGetByID(id);
                if (whoWeUs == null)
                {
                    return NotFound("Silinecek kurumsal bilgi kaydı bulunamadı, kral.");
                }

                // Kayıt bulunduysa iş katmanındaki silme metoduna (TDelete) gönderiyoruz
                _whoWeUsService.TDelete(whoWeUs);

                // Başarılı silme işleminde 200 OK döner
                return Ok("Kurumsal bilgi veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Kurumsal bilgi silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}