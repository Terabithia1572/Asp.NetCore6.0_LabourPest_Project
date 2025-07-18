﻿using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBlogService:IGenericService<Blog>
    {
        List<Blog> GetBlogListWithBlogCategory();      
           List<Blog> GetListWithCategoryByWriter(int id);
        List<Blog> GetLastBlogs(int count);
        List<Blog> GetRecentBlogsByWriter(int writerId, int count);
        List<Blog> GetBlogListWithBlogCategoryAndWriter();
        List<Blog> GetBlogListWithCategoryWriterAndTags();

    }
}
