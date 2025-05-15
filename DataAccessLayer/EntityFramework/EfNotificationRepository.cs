using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfNotificationRepository : GenericRepository<Notification>, INotificationDal
    {
        public List<Notification> GetListByWriter(int writerId)
        {
            using var context = new Context();
            return context.Notifications
                .Where(x => x.WriterID == writerId)
                .Include(x => x.SenderWriter) // 💡 Eklendi
                .OrderByDescending(x => x.NotificationDate)
                .ToList();
        }
        public List<Notification> GetLatestNotificationsByWriter(int writerId, int count = 5)
        {
            using var context = new Context();
            return context.Notifications
                          .Include(x => x.SenderWriter)
                          .Where(x => x.WriterID == writerId)
                          .OrderByDescending(x => x.NotificationDate)
                          .Take(count)
                          .ToList();
        }

    }
}
