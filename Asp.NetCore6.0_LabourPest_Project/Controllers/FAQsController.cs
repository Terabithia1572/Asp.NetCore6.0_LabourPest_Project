using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize]
    public class FAQsController : Controller
    {
        
        FAQManager faqManager= new FAQManager(new EfFAQRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FAQList()
        {
            var values=faqManager.GetAll();
            return View(values);
        }
        // StatusOptions metodunuz (örnek)
        private void StatusOptions()
        {
            ViewBag.StatusOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "1.Sıradaki Sık Kullanılan Sorulara Ekle" },
        new SelectListItem { Value = "0", Text = "2.Sıradaki Sık Kullanılan Sorulara Ekle" }
    };
        }

        [HttpGet]
        public IActionResult AddFAQ()
        {
            // Dropdown için seçenekleri view'a aktar
            StatusOptions();
            return View();
        }

        [HttpPost]
        public IActionResult AddFAQ(FAQ faq, string FAQStatus)
        {
            // Formdan gelen "1" ya da "0" değerini boolean'a çeviriyoruz:
            faq.FAQStatus = FAQStatus == "1";

            faqManager.TAdd(faq);
            return RedirectToAction("FAQList", "FAQ");
        }

        [HttpGet]
        public IActionResult UpdateFAQ(int id)
        {
            var faqValue = faqManager.TGetByID(id);
            // Eğer Update view'ında da dropdown kullanacaksanız:
            StatusOptions();
            return View(faqValue);
        }

        [HttpPost]
        public IActionResult UpdateFAQ(FAQ faq, string FAQStatus)
        {
            // Güncelleme işleminde de değeri dönüştürün:
            faq.FAQStatus = FAQStatus == "1";

            faqManager.TUpdate(faq);
            return RedirectToAction("FAQList", "FAQ");
        }

        public IActionResult DeleteFAQ(int id)
        {
            var faqValue = faqManager.TGetByID(id);
            faqManager.TDelete(faqValue);
            return RedirectToAction("FAQList", "FAQ");
        }

      
    }
}
