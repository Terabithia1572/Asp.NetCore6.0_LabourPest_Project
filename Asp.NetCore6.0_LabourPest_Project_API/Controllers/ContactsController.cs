using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını belirliyoruz: api/Contacts şeklinde çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        // İş katmanındaki Contact servis arayüzünü içeriye enjekte ediyoruz
        private readonly IContactService _contactService;

        // Constructor ile bağımlılığı (Dependency Injection) içeri alıyoruz
        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // =====================================================================================
        // 1. READ (Tüm Mesajları/İletişim Formlarını Listeleme) - GET: api/Contacts
        // =====================================================================================
        [HttpGet]
        public IActionResult GetContactList()
        {
            try
            {
                // İş katmanından tüm iletişim mesajlarını çekiyoruz (Admin panelinde listelemek için)
                var values = _contactService.GetAll();

                // Verileri 200 OK kodu ile birlikte geri dönüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                // Olası bir veri tabanı hatasında 500 kodu ve hata mesajı döner
                return StatusCode(500, $"Mesajlar listelenirken bir hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Mesajı Getirme) - GET: api/Contacts/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetContactById(int id)
        {
            try
            {
                // Gönderilen ID'ye ait mesajı iş katmanından sorguluyoruz
                var value = _contactService.TGetByID(id);

                // Eğer veri tabanında böyle bir mesaj yoksa 404 Bulunamadı döner
                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı mesaj veri tabanında bulunamadı.");
                }

                // Kayıt varsa 200 OK ile veriyi teslim eder
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı mesaj getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Mesaj Gönderme / İletişim Formu) - POST: api/Contacts
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateContact([FromBody] Contact contact)
        {
            try
            {
                // Mobil uygulamadan veya web sitesinden gelen mesaja sunucu tarafında tarih ekleyebilirsin:
                // contact.ContactDate = DateTime.Now; 

                // Gelen mesaj formunu durumunu aktif/okunmadı (true) olarak işaretleyebilirsin:
                // contact.ContactStatus = true;

                // İş katmanındaki TAdd metodunu tetikleyerek mesajı veri tabanına (dbo.Contacts) yazıyoruz
                _contactService.TAdd(contact);

                // Başarılı ekleme durumunda 201 Created kodu ile mesajımızı dönüyoruz
                return StatusCode(201, "Mesajınız aslanlar gibi veri tabanına iletildi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Mesaj gönderilirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Mesaj Bilgisini Güncelleme / Okundu Olarak İşaretleme) - PUT: api/Contacts
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateContact([FromBody] Contact contact)
        {
            try
            {
                // 1. Güncellenmek istenen mesajın gerçekten var olup olmadığını ID ile sorguluyoruz.
                // 🎯 NOT: Senin entity yapındaki birincil anahtar (PK) ismini 'ContactID' kabul ettik.
                var existingContact = _contactService.TGetByID(contact.ContactID);
                if (existingContact == null)
                {
                    return NotFound("Güncellenecek mesaj kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING KORUMASI:
                // Veri tabanından gelen ve takip altında olan nesnenin alanlarını eşitleyerek güncelliyoruz.
                // Tablondaki kolonlara göre buraları düzenleyebilirsin (Örn: Name, Email, Subject, Message, Status):
                existingContact.ContactStatus = contact.ContactStatus; // Mesajı okundu/okunmadı yapmak için

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _contactService.TUpdate(existingContact);

                return Ok("Mesaj bilgisi başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Mesaj güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Mesaj Silme) - DELETE: api/Contacts/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                // Silinmek istenen mesajı ID üzerinden arıyoruz
                var contact = _contactService.TGetByID(id);
                if (contact == null)
                {
                    return NotFound("Silinecek mesaj kaydı bulunamadı, kral.");
                }

                // İş katmanındaki TDelete metoduna göndererek siliyoruz
                _contactService.TDelete(contact);

                return Ok("Mesaj veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Mesaj silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}