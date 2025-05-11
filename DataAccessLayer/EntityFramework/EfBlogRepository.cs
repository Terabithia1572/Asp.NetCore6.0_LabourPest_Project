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
	public class EfBlogRepository : GenericRepository<Blog>, IBlogDal
	{
        public List<Blog> GetListWithBlogCategory()
        {
            using (var context = new Context())
            {
                return context.Blogs
                    .Include(b => b.BlogCategory)
                    .Include(b => b.Writer)
                    .Include(b => b.BlogTags)
                        .ThenInclude(bt => bt.Tag)
                    .ToList();
            }
        }

        public List<Blog> GetListWithCategoryByWriter(int id)
        {
            using (var c = new Context())
            {
                return c.Blogs.Include(x => x.BlogCategory).Where(x => x.WriterID == id).ToList();
            }
        }

        public List<Blog> GetRecentBlogsByWriter(int writerId, int count)
        {
            using (var context = new Context())
            {
                return context.Blogs
                              .Include(b => b.BlogCategory)
                              .Include(b => b.Writer)
                              .Include(b => b.BlogComments)
                              .Where(b => b.WriterID == writerId && b.BlogStatus == true)
                              .OrderByDescending(b => b.BlogDate)
                              .Take(count)
                              .ToList();
            }
        }
        public List<Blog> GetBlogListWithBlogCategoryAndWriter()
        {
            using (var context = new Context())
            {
                return context.Blogs
                   .Include(b => b.BlogCategory)
                   .Include(b => b.Writer) // Writer bilgisini de dahil ediyoruz.
                   .ToList();
            }
        }
    }
}
