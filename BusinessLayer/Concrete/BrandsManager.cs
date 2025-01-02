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
    public class BrandsManager : IBrandsService
    {
        IBrandsDal _brandsDal;

        public BrandsManager(IBrandsDal brandsDal)
        {
            _brandsDal = brandsDal;
        }

        public List<Brands> GetAll()
        {
            return _brandsDal.GetListAll();
        }

        public void TAdd(Brands t)
        {
            _brandsDal.Insert(t);
        }

        public void TDelete(Brands t)
        {
           _brandsDal.Delete(t);
        }

        public Brands TGetByID(int id)
        {
           return _brandsDal.GetByID(id);
        }

        public void TUpdate(Brands t)
        {
           _brandsDal.Update(t);
        }
    }
}
