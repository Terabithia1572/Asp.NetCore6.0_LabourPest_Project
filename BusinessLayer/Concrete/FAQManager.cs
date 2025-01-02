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
    public class FAQManager : IFAQService
    {
        IFAQDal _FAQDal;

        public FAQManager(IFAQDal fAQDal)
        {
            _FAQDal = fAQDal;
        }

        public List<FAQ> GetAll()
        {
            return _FAQDal.GetListAll();
        }

        public void TAdd(FAQ t)
        {
           _FAQDal.Insert(t);
        }

        public void TDelete(FAQ t)
        {
           _FAQDal.Delete(t);
        }

        public FAQ TGetByID(int id)
        {
            return _FAQDal.GetByID(id);
        }

        public void TUpdate(FAQ t)
        {
            _FAQDal.Update(t);
        }
    }
}
