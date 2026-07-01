using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/FAQs şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class FAQsController : ControllerBase
    {
        // İş katmanındaki FAQ servis arayüzünü (Interface) içeriye enjekte ediyoruz
        private readonly IFAQService _faqService;

        // Constructor (Yapıcı Metot) ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public FAQsController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        // =====================================================================================
        // 1. READ (Tüm Sıkça Sorulan Soruları Listeleme) - GET: api/FAQs
        // =====================================================================================
        [HttpGet]
        public IActionResult GetFAQList()
        {
            try
            {
                // İş katmanından tüm SSS (FAQ) verilerini çekiyoruz
                var values = _faqService.GetAll();

                // Verileri 200 OK kodu ile birlikte istemciye (Flutter/Tarayıcı) dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı hatasında 500 kodu ve hata mesajı döner
                return StatusCode(500, $"Sıkça sorulan sorular listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Soru Getirme) - GET: api/FAQs/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetFAQById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait FAQ kaydını iş katmanından sorguluyoruz
                var value = _faqService.TGetByID(id);

                // Eğer veri tabanında böyle bir kayıt yoksa 404 Bulunamadı döner
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı soru veri tabanında bulunamadı.");
                }

                // Kayıt varsa 200 OK ile veriyi teslim eder
                return Ok(value);
            }
            catch (Exception)
            {
                // Katmanlardaki bulunamadı istisnalarına karşı try-catch güvencesi
                return NotFound($"Kral, {id} numaralı soru getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Soru-Cevap Ekleme) - POST: api/FAQs
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateFAQ([FromBody] FAQ faq)
        {
            try
            {
                // Görseldeki 'FAQStatus' alanını yeni kayıt esnasında varsayılan olarak aktif (true) yapıyoruz
                faq.FAQStatus = true;

                // İş katmanındaki TAdd metodunu tetikleyerek yeni soruyu ekliyoruz
                _faqService.TAdd(faq);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Yeni sıkça sorulan soru aslanlar gibi eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Soru eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Soru-Cevap Bilgisini Güncelleme) - PUT: api/FAQs
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateFAQ([FromBody] FAQ faq)
        {
            try
            {
                // 1. Güncellenmek istenen kaydın gerçekten var olup olmadığını ID ile sorguluyoruz.
                // 🎯 Sütun görseline göre nesnenin birincil anahtarı: faq.FAQID
                var existingFAQ = _faqService.TGetByID(faq.FAQID);
                if (existingFAQ == null)
                {
                    return NotFound("Güncellenecek soru kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Gönderdiğin görseldeki ({92FA9C69-CA0E-4F96-9C5F-997BBC336961}.png) gerçek sütun isimleriyle birebir eşliyoruz:
                existingFAQ.FAQTitle = faq.FAQTitle;
                existingFAQ.FAQDescription = faq.FAQDescription;
                existingFAQ.FAQStatus = faq.FAQStatus;

                // Zaten takip altında (Tracked) olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _faqService.TUpdate(existingFAQ);

                return Ok("Sıkça sorulan soru başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Soru güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Soru-Cevap Silme) - DELETE: api/FAQs/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteFAQ(int id)
        {
            try
            {
                // Silinmek istenen kaydı ID üzerinden arıyoruz
                var faq = _faqService.TGetByID(id);
                if (faq == null)
                {
                    return NotFound("Silinecek soru kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna göndererek siliyoruz
                _faqService.TDelete(faq);

                return Ok("Soru veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Soru silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}