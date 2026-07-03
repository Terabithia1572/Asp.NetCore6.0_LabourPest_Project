using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Admins şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        // İş katmanındaki Admin servis arayüzünü içeriye enjekte ediyoruz
        private readonly IAdminService _adminsService;

        // Constructor (Yapıcı Metot) ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public AdminsController(IAdminService adminsService)
        {
            _adminsService = adminsService;
        }

        // =====================================================================================
        // 1. READ (Tüm Yöneticileri Listeleme) - GET: api/Admins
        // =====================================================================================
        [HttpGet]
        public IActionResult GetAdminList()
        {
            try
            {
                // İş katmanından tüm admin verilerini çekiyoruz
                var values = _adminsService.GetAll();

                // Verileri 200 OK kodu ile birlikte istemciye güvenle dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı hatasında 500 kodu ve hata mesajı döner
                return StatusCode(500, $"Yöneticiler listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Yönetici Getirme) - GET: api/Admins/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetAdminById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait admin kaydını iş katmanından sorguluyoruz
                var value = _adminsService.TGetByID(id);

                // Eğer veri tabanında böyle bir yönetici yoksa 404 Bulunamadı döner
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı yönetici veri tabanında bulunamadı.");
                }

                // Kayıt varsa 200 OK ile veriyi teslim eder
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı yönetici getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Yönetici Ekleme) - POST: api/Admins
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateAdmin([FromBody] Admin admin)
        {
            try
            {
                // Yeni eklenen adminin durumunu varsayılan olarak aktif (true) yapıyoruz:
                // admin.AdminStatus = true;

                // İş katmanındaki TAdd metodunu tetikleyerek yeni yöneticiyi ekliyoruz
                _adminsService.TAdd(admin);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Yeni yönetici aslanlar gibi sisteme eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yönetici eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Yönetici Bilgilerini Güncelleme) - PUT: api/Admins
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateAdmin([FromBody] Admin admin)
        {
            try
            {
                // 1. Güncellenmek istenen yöneticinin gerçekten var olup olmadığını ID ile sorguluyoruz.
                // 🎯 NOT: Senin şema yapına göre birincil anahtarı 'AdminID' kabul ettik.
                var existingAdmin = _adminsService.TGetByID(admin.AdminID);
                if (existingAdmin == null)
                {
                    return NotFound("Güncellenecek yönetici kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Veri tabanından gelen ve takip altında (Tracked) olan nesnenin alanlarını eşitleyerek güncelliyoruz.
                // Sütun isimlerini senin gerçek entity yapına göre (Örn: Username, Password, Role vb.) revize edebilirsin kral:
                existingAdmin.Username = admin.Username;
                existingAdmin.Password = admin.Password;
                // existingAdmin.Role = admin.Role; // Varsa yetki rolü alanı

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _adminsService.TUpdate(existingAdmin);

                return Ok("Yönetici bilgileri başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yönetici güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Yönetici Silme) - DELETE: api/Admins/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            try
            {
                // Silinmek istenen kaydı ID üzerinden arıyoruz
                var admin = _adminsService.TGetByID(id);
                if (admin == null)
                {
                    return NotFound("Silinecek yönetici kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna göndererek siliyoruz
                _adminsService.TDelete(admin);

                return Ok("Yönetici sistemden başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Yönetici silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}