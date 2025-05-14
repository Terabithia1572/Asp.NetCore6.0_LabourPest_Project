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
    public class BlogCommentManager : IBlogCommentService
    {
        IBlogCommentDal _blogCommentDal;

        public BlogCommentManager(IBlogCommentDal blogCommentDal)
        {
            _blogCommentDal = blogCommentDal;
        }

        public List<BlogComment> GetAll()
        {
            return _blogCommentDal.GetListAll(x=>x.BlogID==1);
		}
        public List<BlogComment> GetListWithWriterByBlogList(List<int> blogIds)
        {
            return _blogCommentDal.GetListWithWriterByBlogList(blogIds);
        }


        public List<BlogComment> GetCommentListWithBlog()
        {
            return _blogCommentDal.GetCommentListWithBlog();
        }

        public List<BlogComment> GetComments(int id)
		{
			return _blogCommentDal.GetListAll(x => x.BlogID == id);
		}

		public void TAdd(BlogComment t)
        {
            _blogCommentDal.Insert(t);
        }

        public void TDelete(BlogComment t)
        {
            _blogCommentDal.Delete(t);
        }

        public BlogComment TGetByID(int id)
        {
            return _blogCommentDal.GetByID(id);
        }

        public void TUpdate(BlogComment t)
        {
            _blogCommentDal.Update(t);
        }
    }
}
