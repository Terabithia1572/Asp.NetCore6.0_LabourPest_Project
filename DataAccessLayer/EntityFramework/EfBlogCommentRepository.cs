using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfBlogCommentRepository: GenericRepository<BlogComment>,IBlogCommentDal
    {
        public List<BlogComment> GetCommentListWithBlog()
        {
            using (var context = new Context())
            {
                return context.BlogsComments.Include(x => x.Blog).ToList();
            }
        }
        public List<BlogComment> GetListWithWriterByBlogList(List<int> blogIds)
        {
            using var context = new Context();
            return context.BlogsComments
                          .Include(x => x.Writer)
                          .Where(x => blogIds.Contains(x.BlogID))
                          .ToList();
        }

    }
}
