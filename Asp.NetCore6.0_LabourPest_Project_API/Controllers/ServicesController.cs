using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Services şeklinde çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        // İş katmanındaki servis arayüzümüzü enjekte etmek için private alan tanımlıyoruz
        private readonly IServicesService _servicesService;

        // Constructor (Yapıcı Metot) ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        // =====================================================================================
        // 1. READ (Tüm Hizmetleri Listeleme) - GET: api/Services
        // =====================================================================================
        [HttpGet]
        public IActionResult GetServiceList()
        {
            try
            {
                // İş katmanından tüm hizmet verilerini çekiyoruz
                var values = _servicesService.GetAll();

                // Verileri 200 OK kodu ile birlikte istemciye (Flutter/Tarayıcı) dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı veya sistem hatasında 500 hata kodu ve mesajı döner
                return StatusCode(500, $"Hizmetler listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Hizmet Getirme) - GET: api/Services/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetServiceById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait hizmeti iş katmanından sorguluyoruz
                var value = _servicesService.TGetByID(id);

                // Eğer veri tabanında böyle bir kayıt yoksa 404 Bulunamadı döner
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı hizmet veri tabanında bulunamadı.");
                }

                // Kayıt varsa 200 OK ile veriyi teslim eder
                return Ok(value);
            }
            catch (Exception)
            {
                // Katmanlardaki olası bulunamadı hatalarına karşı try-catch koruması
                return NotFound($"Kral, {id} numaralı hizmet getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Hizmet Ekleme) - POST: api/Services
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateService([FromBody] Services service)
        {
            try
            {
                // Yeni eklenen hizmetin durumunu varsayılan olarak aktif (true) yapıyoruz
                service.ServicesStatus = true;

                // İş katmanındaki TAdd metodunu tetikleyerek kaydı veri tabanına ekliyoruz
                _servicesService.TAdd(service);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Yeni hizmet aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                // Ekleme esnasında bir sorun çıkarsa 400 Bad Request fırlatır
                return BadRequest($"Hizmet eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Mevcut Hizmeti Güncelleme) - PUT: api/Services
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateService([FromBody] Services service)
        {
            try
            {
                // 1. Güncellenmek istenen asıl kaydı veri tabanından mevcut ID ile çekiyoruz.
                // Bu işlem nesneyi EF Core üzerinde otomatik olarak takibe (Tracking) alır.
                var existingService = _servicesService.TGetByID(service.ServicesID);
                if (existingService == null)
                {
                    return NotFound("Güncellenecek hizmet kaydı bulunamadı, kral.");
                }

                // 🎯 TEK KAREDE KESİN ÇÖZÜM:
                // EF Core aynı ID'ye sahip iki farklı nesneyi eş zamanlı takip edemediği için;
                // Swagger'dan gelen 'service' nesnesini doğrudan güncellemek yerine, veri tabanından
                // gelen ve zaten takipte olan 'existingService' nesnesinin kolonlarını güncelliyoruz!
                existingService.ServicesTitle = service.ServicesTitle;
                existingService.ServicesSlug = service.ServicesSlug;
                existingService.ServicesDescription = service.ServicesDescription;
                existingService.ServicesLongDescription = service.ServicesLongDescription;
                existingService.ServicesImageURL = service.ServicesImageURL;
                existingService.ServicesStatus = service.ServicesStatus;

                // Zaten takip altında (Tracked) olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _servicesService.TUpdate(existingService);

                // İşlem başarılıysa 200 OK yanıtını basıyoruz
                return Ok("Hizmet bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                // Çakışma veya veri uyuşmazlığı durumunda yakalayıp ekrana basıyoruz
                return BadRequest($"Hizmet güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Hizmet Silme) - DELETE: api/Services/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            try
            {
                // Silinmek istenen kaydı ID üzerinden veri tabanında arıyoruz
                var service = _servicesService.TGetByID(id);
                if (service == null)
                {
                    return NotFound("Silinecek hizmet kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna bulduğumuz nesneyi göndererek siliyoruz
                _servicesService.TDelete(service);

                // Başarılı silme durumunda 200 OK yanıtı döner
                return Ok("Hizmet veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hizmet silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}