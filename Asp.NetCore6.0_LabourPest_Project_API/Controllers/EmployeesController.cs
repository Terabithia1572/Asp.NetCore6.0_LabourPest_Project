using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project_API.Controllers
{
    // API rotasını ekiyoruz: api/Employees şeklinde dış dünyadan çağrılacak
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        // İş katmanındaki Employees servis arayüzünü içeriye enjekte ediyoruz
        private readonly IEmployeeService _employeesService;

        // Constructor (Yapıcı Metot) vasıtasıyla bağımlılığı içeri alıyoruz
        public EmployeesController(IEmployeeService employeesService)
        {
            _employeesService = employeesService;
        }

        // =====================================================================================
        // 1. READ (Tüm Çalışanları Listeleme) - GET: api/Employees
        // =====================================================================================
        [HttpGet]
        public IActionResult GetEmployeeList()
        {
            try
            {
                // İş katmanı üzerinden tüm personel verilerini çekiyoruz
                var values = _employeesService.GetAll();

                // Başarılıysa verileri 200 OK kodu ile birlikte geri döndürüyoruz
                return Ok(values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Personel listesi çekilirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 2. READ (ID Değerine Göre Tek Bir Çalışanı Getirme) - GET: api/Employees/5
        // =====================================================================================
        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                var value = _employeesService.TGetByID(id);

                if (value == null)
                {
                    return NotFound($"Kral, {id} numaralı personel veri tabanında bulunamadı.");
                }

                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound($"Kral, {id} numaralı personel getirilirken katman korumasına takıldı.");
            }
        }

        // =====================================================================================
        // 3. CREATE (Yeni Personel Ekleme) - POST: api/Employees
        // =====================================================================================
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                // 🎯 DOĞRULAMA: Gönderdin şemaya göre durum bilgisini aktif (true) yapıyoruz
                employee.EmployeeStatus = true;

                _employeesService.TAdd(employee);
                return StatusCode(201, "Yeni personel aslanlar gibi sisteme eklendi, kral!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Personel eklenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 4. UPDATE (Personel Bilgilerini Güncelleme) - PUT: api/Employees
        // =====================================================================================
        [HttpPut]
        public IActionResult UpdateEmployee([FromBody] Employee employee)
        {
            try
            {
                // 1. Güncellenmek istenen asıl kaydı veri tabanından mevcut ID ile çekip takibe alıyoruz.
                // 🎯 DOĞRULAMA: Birincil anahtar alanı 'EmployeeID' olarak eşitlendi.
                var existingEmployee = _employeesService.TGetByID(employee.EmployeeID);
                if (existingEmployee == null)
                {
                    return NotFound("Güncellenecek personel kaydı bulunamadı, kral.");
                }

                // 🎯 EF CORE TRACKING ÇAKIŞMASI KESİN ÇÖZÜMÜ:
                // Sütun isimlerini senin gönderdiğin net listeye göre birebir eşitliyoruz kral:
                existingEmployee.EmployeeName = employee.EmployeeName;
                existingEmployee.EmployeeSurName = employee.EmployeeSurName;
                existingEmployee.EmployeeImage = employee.EmployeeImage;
                existingEmployee.EmployeeTwitter = employee.EmployeeTwitter;
                existingEmployee.EmployeeFacebook = employee.EmployeeFacebook;
                existingEmployee.EmployeeInstagram = employee.EmployeeInstagram;
                existingEmployee.EmployeeLinkedin = employee.EmployeeLinkedin;
                existingEmployee.EmployeeStatus = employee.EmployeeStatus;

                // Zaten takip altında olan nesneyi iş katmanına güncellenmesi için gönderiyoruz
                _employeesService.TUpdate(existingEmployee);

                return Ok("Personel bilgileri başarıyla güncellendi, kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Personel güncellenirken hata oluştu kral: {ex.Message}");
            }
        }

        // =====================================================================================
        // 5. DELETE (Personel Silme) - DELETE: api/Employees/5
        // =====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                var employee = _employeesService.TGetByID(id);
                if (employee == null)
                {
                    return NotFound("Silinecek personel kaydı bulunamadı, kral.");
                }

                _employeesService.TDelete(employee);
                return Ok("Personel veri tabanından başarıyla silindi kral.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Personel silinirken bir hata meydana geldi: {ex.Message}");
            }
        }
    }
}