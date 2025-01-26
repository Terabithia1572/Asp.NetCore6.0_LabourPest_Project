using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminMailController : Controller
    {
        MailManager mailManager = new MailManager(new EfMailRepository());
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MailList()
        {
            var values = mailManager.GetAll();
            return View(values);
        }
        public IActionResult MailLogin()
        {
            return View();
        }
        public IActionResult MailReply(int id)
        {
            var mail = mailManager.TGetByID(id);
            if (mail == null)
            {
                return NotFound();
            }

            // Writer tablosundan kullanıcıları çek
            var writers = writerManager.GetAll()
                .Select(w => new
                {
                    w.WriterMail,
                    w.WriterName,
                    w.WriterSurname
                })
                .ToList();

            ViewBag.Writers = writers;

            return View(mail);
        }


        public IActionResult GetMailDetail(int id)
        {
            var mail = mailManager.TGetByID(id);
            if (mail == null)
            {
                return NotFound();
            }

            return PartialView("_MailDetailPartial", mail);
        }
        [HttpPost]
        public IActionResult SendReply(string ReceiverMail, string MailName, string MailSurname, string MailTitle, string MailContent)
        {
            if (string.IsNullOrEmpty(ReceiverMail) || string.IsNullOrEmpty(MailName) ||
                string.IsNullOrEmpty(MailSurname) || string.IsNullOrEmpty(MailTitle) || string.IsNullOrEmpty(MailContent))
            {
                TempData["Error"] = "Lütfen tüm alanları doldurunuz!";
                return RedirectToAction("MailReply");
            }

            Mail newMail = new Mail
            {
                ReceiverMail = ReceiverMail,
                MailName = MailName,
                MailSurname = MailSurname,
                MailTitle = MailTitle,
                MailContent = MailContent,
                MailDate = DateTime.Now
            };

            mailManager.TAdd(newMail);

            TempData["Success"] = "Mesaj başarıyla gönderildi!";
            return RedirectToAction("MailList");
        }
        



    }
}
