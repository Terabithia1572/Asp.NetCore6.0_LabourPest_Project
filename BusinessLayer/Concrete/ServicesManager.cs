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
    public class ServicesManager : IServicesService
    {
        IServicesDal _servicesDal;

        public ServicesManager(IServicesDal servicesDal)
        {
            _servicesDal = servicesDal;
        }

        public List<Services> GetAll()
        {
            return _servicesDal.GetListAll();
        }

        public void TAdd(Services t)
        {
            _servicesDal.Insert(t);
        }

        public void TDelete(Services t)
        {
            _servicesDal.Delete(t);
        }

        public Services TGetByID(int id)
        {
            return _servicesDal.GetByID(id);
        }

        public void TUpdate(Services t)
        {
            _servicesDal.Update(t);
        }
    }
}
