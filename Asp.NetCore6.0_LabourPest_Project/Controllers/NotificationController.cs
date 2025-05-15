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

        [HttpGet]
        public IActionResult GetNotificationComponent()
        {
            return ViewComponent("NotificationList");
        }

    }

}
