using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.CustomerDashboardLayout
{
    public class NotificationList : ViewComponent
    {
        NotificationManager notificationManager = new NotificationManager(new EfNotificationRepository());

        public IViewComponentResult Invoke()
        {
            var userIdRaw = UserClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdRaw))
            {
                // Bu, tarayıcıdan AJAX gelince çalışmazsa, sebep budur
                return Content("⚠️ ViewComponent içinde kullanıcı bilgisi yok.");
            }

            int writerId = Convert.ToInt32(userIdRaw);

            var values = notificationManager.GetLatestNotificationsByWriter(writerId, 5)
                                            //.Where(x => x.NotificationStatus == false)
                                            .ToList();

            return View(values);
        }
    }



}

