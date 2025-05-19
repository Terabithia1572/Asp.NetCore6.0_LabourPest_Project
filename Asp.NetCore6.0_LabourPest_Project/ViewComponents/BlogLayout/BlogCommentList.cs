using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.BlogLayout
{
    public class BlogCommentList : ViewComponent
    {
        BlogCommentManager blogCommentManager = new BlogCommentManager(new EfBlogCommentRepository());

        public IViewComponentResult Invoke(int id)
        {
            var flatComments = blogCommentManager.GetComments(id);

            // Ağaç yapısı oluşturuluyor
            var commentTree = flatComments
     .Where(c => c.ParentCommentID == null)
     .Select(c => new CommentTreeViewModel
     {
         CommentID = c.BlogCommentID,
         AuthorName = c.BlogCommentUserName,
         Content = c.BlogCommentContent,
         Title = c.BlogCommentTitle,
         CommentDate = c.BlogCommentDate,
         AuthorImage = c.BlogImageUrl,
         WriterID = c.WriterID ?? 0, // 🔴 BU SATIR EKLENMELİ
         ParentCommentID = null,
         Replies = flatComments
             .Where(r => r.ParentCommentID == c.BlogCommentID)
             .Select(r => new CommentTreeViewModel
             {
                 CommentID = r.BlogCommentID,
                 AuthorName = r.BlogCommentUserName,
                 Content = r.BlogCommentContent,
                 Title = r.BlogCommentTitle,
                 CommentDate = r.BlogCommentDate,
                 AuthorImage = r.BlogImageUrl,
                 WriterID = r.WriterID ?? 0, // 🔴 BU DA EKLENMELİ
                 ParentCommentID = r.ParentCommentID
             }).ToList()
     }).ToList();


            return View(commentTree);
        }
    }
}
