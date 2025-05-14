using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBlogCommentService:IGenericService<BlogComment>
    {
        List<BlogComment> GetComments(int id);
        List<BlogComment> GetCommentListWithBlog();
        List<BlogComment> GetListWithWriterByBlogList(List<int> blogIds);

    }
}
