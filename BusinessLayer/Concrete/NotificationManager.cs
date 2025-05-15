using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly INotificationDal _notificationDal;

        public NotificationManager(INotificationDal notificationDal)
        {
            _notificationDal = notificationDal;
        }

        public List<Notification> GetListByWriter(int writerId)
        {
            return _notificationDal.GetListByWriter(writerId);
        }
        public List<Notification> GetAll()
        {
            return _notificationDal.GetListAll();
        }
        public List<Notification> GetLatestNotificationsByWriter(int writerId, int count = 5)
        {
            return _notificationDal.GetLatestNotificationsByWriter(writerId, count);
        }

        public void TAdd(Notification notification)
        {
            _notificationDal.Insert(notification); // BU metodun içinde SaveChanges olmalı
        }

        public void TDelete(Notification t)
        {
            _notificationDal.Delete(t);
        }

        public Notification TGetByID(int id)
        {
            return _notificationDal.GetByID(id);
        }

        public List<Notification> TGetList()
        {
            return _notificationDal.GetListAll();
        }

        public void TUpdate(Notification t)
        {
            _notificationDal.Update(t);
        }
    }
}
