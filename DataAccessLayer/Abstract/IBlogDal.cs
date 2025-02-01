using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IBlogDal:IGenericDal<Blog>
    {
        List<Blog> GetListWithBlogCategory();
        List<Blog> GetListWithCategoryByWriter(int id);
        List<Blog> GetRecentBlogsByWriter(int writerId, int count);
    }
}
