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
    public class BlogManager : IBlogService
    {
        IBlogDal _blogDal;

        public BlogManager(IBlogDal blogDal)
        {
            _blogDal = blogDal;
        }

        public List<Blog> GetAll()
        {
            return _blogDal.GetListAll();
        }

        public List<Blog> GetBlogListWithBlogCategory()
        {
            return _blogDal.GetListWithBlogCategory();
        }

		public List<Blog> GetLastBlogs(int count)
		{
			return _blogDal.GetListAll()
				.OrderByDescending(b => b.BlogDate) // Tarihe göre sıralama
				.Take(count) // Son X blog
				.ToList();
		}

		public void TAdd(Blog t)
        {
            _blogDal.Insert(t);
        }

        public void TDelete(Blog t)
        {
            _blogDal.Delete(t);
        }

        public Blog TGetByID(int id) // Blog detayını getiren metot bunu güncellemede kullanacağım
		{
            return _blogDal.GetByID(id);
        }
        public List<Blog> GetBlogByID(int id)
		{
			return _blogDal.GetListAll(x => x.BlogID == id);
		}

		public void TUpdate(Blog t)
        {
            _blogDal.Update(t);
        }
    }
}
