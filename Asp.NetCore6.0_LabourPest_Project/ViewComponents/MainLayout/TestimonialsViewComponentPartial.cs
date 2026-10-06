using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Asp.NetCore6._0_LabourPest_Project.Presentation;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
	public class TestimonialsViewComponentPartial:ViewComponent
	{
		CommentManager commentManager = new(new EfCommentRepository()); 
		public IViewComponentResult Invoke()
		{
            // Restore the existing Comments read path. CommentStatus is not an approval flag;
            // the existing repository has no visibility predicate and deletion removes records.
            var values = commentManager.GetAll()
                .OrderByDescending(comment => comment.CommentDate)
                .ThenByDescending(comment => comment.CommentID)
                .Take(PublicReviews.MaximumCount)
                .ToList();
            return View(values);
		}
	}
}
