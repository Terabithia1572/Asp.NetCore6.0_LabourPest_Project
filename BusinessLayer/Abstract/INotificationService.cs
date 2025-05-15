using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface INotificationService : IGenericService<Notification>
    {
        List<Notification> GetListByWriter(int writerId);
        List<Notification> GetLatestNotificationsByWriter(int writerId, int count = 5);
    }
}
