using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NotificationManager _notificationManager = new NotificationManager(new EfNotificationRepository());
        private readonly WriterManager _writerManager = new WriterManager(new EfWriterRepository());

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetLatestNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var values = _notificationManager.GetLatestNotificationsByWriter(writerId, 5);

            return PartialView("~/Views/Shared/Components/NotificationList/_NotificationDropdown.cshtml", values);
        }

        [Authorize] // Mutlaka bu olmalı
        [HttpGet]
        public IActionResult GetNotificationComponent()
        {
            return ViewComponent("NotificationList");
        }
        [HttpGet]
        public IActionResult GetLatestNotificationsJson()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var notifications = _notificationManager.GetListByWriter(writerId)
                .OrderByDescending(x => x.NotificationDate)
                .Take(5)
                .Select(x => new
                {
                    x.NotificationMessage,
                    NotificationDate = x.NotificationDate.ToString("g"),
                    senderName = x.SenderWriter?.WriterName + " " + x.SenderWriter?.WriterSurname,
                    x.NotificationUrl
                }).ToList();

            return Json(notifications);
        }



    }

}
