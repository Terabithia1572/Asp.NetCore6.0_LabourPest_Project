using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class FAQViewComponentPartial:ViewComponent
	{
		FAQManager fAQManager = new FAQManager(new EfFAQRepository());
		
        public IViewComponentResult Invoke()
		{
            var allFAQs = fAQManager.GetAll();

            // Kategorilere göre ayır
            var accordion01List = allFAQs.Where(f => f.FAQStatus == true).ToList();
            var accordion02List = allFAQs.Where(f => f.FAQStatus == false).ToList();

            // ViewData veya bir ViewModel kullanarak verileri gönder
            ViewData["Accordion01"] = accordion01List;
            ViewData["Accordion02"] = accordion02List;

            
            return View();
		}
	}
}
