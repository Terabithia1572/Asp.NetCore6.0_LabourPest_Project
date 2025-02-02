using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.CustomerDashboardLayout
{
    public class _AdminTopLogoViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                // Giriş yapılmamışsa veya id bulunamazsa boş model gönderilebilir.
                return View(null);
            }
            int writerId = Convert.ToInt32(userIdString);

            // WriterManager üzerinden ilgili yazar bilgilerini çekiyoruz.
            WriterManager writerManager = new WriterManager(new EfWriterRepository());
            Writer writer = writerManager.TGetByID(writerId);

            return View(writer);
        }
    }
}
