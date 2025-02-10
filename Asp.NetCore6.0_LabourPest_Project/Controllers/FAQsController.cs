using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public IActionResult AddFAQ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddFAQ(FAQ faq)
        {
            faqManager.TAdd(faq);
            return RedirectToAction("FAQList","FAQ");
        }
        [HttpGet]
        public IActionResult UpdateFAQ(int id)
        {
            var faqValue = faqManager.TGetByID(id);
            return View(faqValue);
        }
        [HttpPost]
        public IActionResult UpdateFAQ(FAQ faq)
        {
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
