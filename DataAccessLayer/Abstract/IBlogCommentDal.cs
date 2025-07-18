﻿using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IBlogCommentDal:IGenericDal<BlogComment>
    {
        List<BlogComment> GetCommentListWithBlog();
        List<BlogComment> GetListWithWriterByBlogList(List<int> blogIds);

    }
}
