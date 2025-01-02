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
    public class WhoWeUsManager : IWhoWeUsService
    {
        IWhoWeUsDal _whoWeUsDal;

        public WhoWeUsManager(IWhoWeUsDal whoWeUsDal)
        {
            _whoWeUsDal = whoWeUsDal;
        }

        public List<WhoWeUs> GetAll()
        {
            return _whoWeUsDal.GetListAll();
        }

        public void TAdd(WhoWeUs t)
        {
            _whoWeUsDal.Insert(t);
        }

        public void TDelete(WhoWeUs t)
        {
            _whoWeUsDal.Delete(t);
        }

        public WhoWeUs TGetByID(int id)
        {
            return _whoWeUsDal.GetByID(id);
        }

        public void TUpdate(WhoWeUs t)
        {
            _whoWeUsDal.Update(t);
        }
    }
}
