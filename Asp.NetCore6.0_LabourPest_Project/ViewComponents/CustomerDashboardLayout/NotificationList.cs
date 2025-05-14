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
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            var values = notificationManager.GetListByWriter(writerId)
                                            .Where(x => x.NotificationStatus == false)
                                            .Take(5)
                                            .ToList();

            return View(values);
        }

    }
}
