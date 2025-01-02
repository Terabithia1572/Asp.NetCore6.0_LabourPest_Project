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
    public class LogManager : ILogService
    {
        ILogDal _logDal;

        public LogManager(ILogDal logDal)
        {
            _logDal = logDal;
        }

        public List<Log> GetAll()
        {
            return _logDal.GetListAll();
        }

        public void TAdd(Log t)
        {
            _logDal.Insert(t);
        }

        public void TDelete(Log t)
        {
            _logDal.Delete(t);
        }

        public Log TGetByID(int id)
        {
            return _logDal.GetByID(id);
        }

        public void TUpdate(Log t)
        {
            _logDal.Update(t);
        }
    }
}
