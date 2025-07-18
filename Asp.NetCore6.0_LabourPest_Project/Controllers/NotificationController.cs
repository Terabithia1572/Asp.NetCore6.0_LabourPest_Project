﻿using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
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
        Context _context = new Context();

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
                   x.NotificationID,
                   x.NotificationMessage,
                   NotificationDate = x.NotificationDate.ToString("g"),
                   senderName = x.SenderWriter?.WriterName + " " + x.SenderWriter?.WriterSurname,
                   senderImage = x.SenderWriter?.WriterImage ?? "/default-user.png",
                   x.NotificationStatus,
                   x.NotificationUrl
               }).ToList();

            return Json(notifications);
        }
        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var notifications = _notificationManager.GetListByWriter(writerId)
                .Where(x => !x.NotificationStatus).ToList();

            foreach (var item in notifications)
            {
                item.NotificationStatus = true;
                _notificationManager.TUpdate(item);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteAllNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var notifications = _notificationManager.GetListByWriter(writerId).ToList();

            foreach (var notification in notifications)
            {
                _notificationManager.TDelete(notification);
            }

            return Ok();
        }

        public IActionResult GetFilteredNotifications(string status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var notifications = _notificationManager.GetListByWriter(writerId)
                .Where(n => status == "all"
                            || (status == "read" && n.NotificationStatus)
                            || (status == "unread" && !n.NotificationStatus))
                .OrderByDescending(n => n.NotificationDate)
                .Select(n => new {
                    n.NotificationID,
                    n.NotificationMessage,
                    NotificationDate = n.NotificationDate.ToString("g"),
                    n.NotificationType,
                    senderName = n.SenderWriter?.WriterName + " " + n.SenderWriter?.WriterSurname,
                    senderImage = n.SenderWriter?.WriterImage ?? "/default-user.png",
                    n.NotificationStatus,
                    n.NotificationUrl
                }).ToList();

            return Json(notifications);
        }
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
            {
                notification.NotificationStatus = true;
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
            return Ok();
        }



    }

}
