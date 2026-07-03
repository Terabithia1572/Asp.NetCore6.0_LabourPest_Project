using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/JobApplications şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        // İş katmanındaki JobApplication servis arayüzünü içeriye enjekte ediyoruz
        private readonly IJobApplicationService _jobApplicationService;

        // Constructor (Yapıcı Metot) ile bağımlılığı içeri alıyoruz
        public JobApplicationsController(IJobApplicationService jobApplicationService)
        {
            _jobApplicationService = jobApplicationService;
        }

        // =====================================================================================
        // 1. READ (Tüm İş Başvurularını Listeleme) - GET: api/JobApplications
        // =====================================================================================
        [HttpGet]
        public IActionResult GetJobApplicationList()
        {
            try
            {
                // İş katmanı üzerinden gelen tüm iş başvurularını çekiyoruz (Admin paneli için)
                var values = _jobApplicationService.GetAll();

                // Başarılıysa verileri 200 OK kodu ile geri döndürüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Sistemde beklenmeyen bir hata çıkarsa 500 koduyla hata mesajını basıyoruz
                return StatusCode(500, $"İş başvuruları listelenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Başvuru Getirme) - GET: api/JobApplications/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetJobApplicationById(int id)
        {
            try
            {
                // Gelen ID numarasına göre veri tabanında tekil arama yapıyoruz
                var value = _jobApplicationService.TGetByID(id);

                // Eğer böyle bir başvuru yoksa istemciye 404 Bulunamadı dönüyoruz
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı iş başvurusu bulunamadı.");
                }

                // Kayıt mevcutsa veriyi 200 OK ile teslim ediyoruz
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı başvuru getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni İş Başvurusu Yapma) - POST: api/JobApplications
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateJobApplication([FromBody] JobApplication jobApplication)
        {
            try
            {
                // Eğer veri tabanında başvuru durumu (Status) varsa varsayılan olarak aktif/değerlendirmede (true) yapıyoruz:
                // jobApplication.JobApplicationStatus = true;

                // İş katmanındaki Add (TAdd) metoduna nesnemizi gönderip SQL'e yazdırıyoruz
                _jobApplicationService.TAdd(jobApplication);

                // Başarılı bir şekilde eklenince 201 Created durum kodu ve mesajımızı veriyoruz
                return StatusCode(201, "İş başvurusu aslanlar gibi veri tabanına eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Başvuru yapılırken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Başvuru Bilgisini Güncelleme / Durum Değişikliği) - PUT: api/JobApplications
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateJobApplication([FromBody] JobApplication jobApplication)
        {
            try
            {
                // 1. Güncellenmek istenen asıl kaydı veri tabanından mevcut ID ile çekiyoruz.
                // 🎯 NOT: Senin entity yapındaki birincil anahtar (PK) ismini 'JobApplicationID' kabul ettik.
                var existingApplication = _jobApplicationService.TGetByID(jobApplication.JobApplicationID);
                if (existingApplication == null)
                {
                    return NotFound("Güncellenecek başvuru kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Veri tabanından gelen ve takip altında (Tracked) olan nesnenin alanlarını tek tek eşitliyoruz.
                // Sütun isimlerini senin veri tabanına göre (Örn: Name, Surname, CVPath, Status vb.) revize edebilirsin kral:
                existingApplication.JobApplicationStatus = jobApplication.JobApplicationStatus;
                // existingApplication.JobApplicationNotes = jobApplication.JobApplicationNotes; // Varsa İK notları

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _jobApplicationService.TUpdate(existingApplication);

                return Ok("İş başvurusu bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Başvuru güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (İş Başvurusunu Silme) - DELETE: api/JobApplications/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteJobApplication(int id)
        {
            try
            {
                // Silinmek istenen kaydı önce ID parametresi üzerinden kontrol ediyoruz
                var jobApplication = _jobApplicationService.TGetByID(id);
                if (jobApplication == null)
                {
                    return NotFound("Silinecek başvuru kaydı bulunamadı, kral.");
                }

                // Kayıt bulunduysa iş katmanındaki silme metoduna (TDelete) gönderiyoruz
                _jobApplicationService.TDelete(jobApplication);

                return Ok("İş başvurusu veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Başvuru silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}