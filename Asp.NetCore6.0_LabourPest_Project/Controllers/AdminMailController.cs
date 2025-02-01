using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin,Müşteri")]
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
            string userEmail = User.Identity.Name;

            // Tüm maillerden sadece giriş yapan kullanıcıya gönderilmiş olanları filtreleyin.
            var values = mailManager.GetAll()
                                    .Where(m => m.ReceiverMail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

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

            // Giriş yapan kullanıcının e-posta bilgisini alıyoruz.
            string userEmail = User.Identity.Name;

            // Writer tablosundan giriş yapanın bilgilerini çekiyoruz.
            var writer = writerManager.GetAll()
                .FirstOrDefault(w => w.WriterMail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            if (writer != null)
            {
                // Bu bilgileri view’da kullanmak üzere ViewBag üzerinden gönderiyoruz.
                ViewBag.CurrentWriterName = writer.WriterName;
                ViewBag.CurrentWriterSurname = writer.WriterSurname;
            }

            /* 
               Eğer gelen mailin gönderici bilgilerini de saklıyorsanız (örneğin SenderMail adında bir property varsa)
               bu bilgiyi de view’a gönderebilir ve "Kime" alanında otomatik seçili hale getirebilirsiniz.
               Aşağıdaki örnekte mail modelinizde "SenderMail" olduğunu varsayıyoruz.
            */

            // Tüm writer listesini yine drop-down için gönderelim.
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

            // Giriş yapan kullanıcının mail adresini alıyoruz.
            // Eğer User.Identity.Name yazıyı içeriyorsa doğrudan kullanabilirsiniz, yoksa WriterManager'dan çekebilirsiniz.
            string senderMail = User.Identity.Name;

            Mail newMail = new Mail
            {
                ReceiverMail = ReceiverMail,
                SenderMail = senderMail, // Gönderen bilgisi
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
        public IActionResult SentMailList()
        {
            // Giriş yapan kullanıcının mail adresini alıyoruz.
            string userEmail = User.Identity.Name;

            // Tüm maillerden, gönderici adresi oturum açan kullanıcıyla eşleşenleri çekiyoruz.
            var values = mailManager.GetAll()
                                    .Where(m => m.SenderMail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

            // MailList.cshtml görünümünü kullanıyoruz (tasarım aynı olacak).
            return View("MailList", values);
        }




    }
}
