using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface INotificationDal : IGenericDal<Notification>
    {
        List<Notification> GetListByWriter(int writerId); // yazar bazlı bildirimler
        List<Notification> GetLatestNotificationsByWriter(int writerId, int count = 5);
    }
}
