﻿using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public List<Product> GetAll()
        {
            return _productDal.GetListAll();
        }

        public List<Product> GetListWithProductCategory()
        {
           return _productDal.GetListWithProductCategory();
        }

        public void TAdd(Product t)
        {
            _productDal.Insert(t);
        }

        public void TDelete(Product t)
        {
           _productDal.Delete(t);
        }

        public Product TGetByID(int id)
        {
            return _productDal.GetByID(id);
        }

        public void TUpdate(Product t)
        {
            _productDal.Update(t);
        }
    }
}
