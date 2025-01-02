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
    public class ImageManager : IImageService
    {
        IImageDal _imageDal;

        public ImageManager(IImageDal imageDal)
        {
            _imageDal = imageDal;
        }

        public List<Image> GetAll()
        {
            return _imageDal.GetListAll();
        }

        public void TAdd(Image t)
        {
            _imageDal.Insert(t);
        }

        public void TDelete(Image t)
        {
            _imageDal.Delete(t);
        }

        public Image TGetByID(int id)
        {
            return _imageDal.GetByID(id);
        }

        public void TUpdate(Image t)
        {
            _imageDal.Update(t);
        }
    }
}
