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
    public class MailManager : IMailService
    {
        IMailDal _mailDal;

        public MailManager(IMailDal mailDal)
        {
            _mailDal = mailDal;
        }

        public List<Mail> GetAll()
        {
           return _mailDal.GetListAll();
        }

        public List<Mail> GetMailByID(int id)
        {
            return _mailDal.GetListAll(x=>x.MailID == id);
        }

        public void TAdd(Mail t)
        {
            _mailDal.Insert(t);
        }

        public void TDelete(Mail t)
        {
            _mailDal.Delete(t);
        }

        public Mail TGetByID(int id)
        {
            return _mailDal.GetByID(id);
        }

        public void TUpdate(Mail t)
        {
            _mailDal.Update(t);
        }
    }
}
