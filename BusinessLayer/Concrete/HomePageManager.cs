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
    public class HomePageManager : IHomePageService
    {
        IHomePageDal _homePageDal;

        public HomePageManager(IHomePageDal homePageDal)
        {
            _homePageDal = homePageDal;
        }

        public List<HomePage> GetAll()
        {
            return _homePageDal.GetListAll();
        }

        public void TAdd(HomePage t)
        {
            _homePageDal.Insert(t);
        }

        public void TDelete(HomePage t)
        {
            _homePageDal.Delete(t);
        }

        public HomePage TGetByID(int id)
        {
            return _homePageDal.GetByID(id);
        }

        public void TUpdate(HomePage t)
        {
            _homePageDal.Update(t);
        }
    }
}
