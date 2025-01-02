using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class SubscribeManager : ISubscribeService
    {
        ISubscribeDal _subscribeDal;

        public SubscribeManager(ISubscribeDal subscribeDal)
        {
            _subscribeDal = subscribeDal;
        }

        public List<Subscribe> GetAll()
        {
            return _subscribeDal.GetListAll();
        }

        public void TAdd(Subscribe t)
        {
            _subscribeDal.Insert(t);
        }

        public void TDelete(Subscribe t)
        {
            _subscribeDal.Delete(t);
        }

        public Subscribe TGetByID(int id)
        {
            return _subscribeDal.GetByID(id);
        }

        public void TUpdate(Subscribe t)
        {
            _subscribeDal.Update(t);
        }
    }
}
