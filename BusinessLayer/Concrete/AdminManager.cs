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
    public class AdminManager : IAdminService
    {
        IAdminDal _adminDal;

        public AdminManager(IAdminDal adminDal)
        {
            _adminDal = adminDal;
        }

        public List<Admin> GetAdminByID(int id)
        {
            return _adminDal.GetListAll(x=>x.AdminID == id);
        }

        public List<Admin> GetAll()
        {
            return _adminDal.GetListAll();
        }

        public void TAdd(Admin t)
        {
            _adminDal.Insert(t);
        }

        public void TDelete(Admin t)
        {
            _adminDal.Delete(t);
        }

        public Admin TGetByID(int id)
        {
            return _adminDal.GetByID(id);
        }

        public void TUpdate(Admin t)
        {
           _adminDal.Update(t);
        }
    }
}
