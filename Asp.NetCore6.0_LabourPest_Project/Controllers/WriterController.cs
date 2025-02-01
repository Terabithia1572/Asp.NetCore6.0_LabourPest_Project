using Asp.NetCore6._0_LabourPest_Project.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class WriterController : Controller
    {
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        BlogManager blogManager = new BlogManager(new EfBlogRepository());
        Context context = new Context();
        public IActionResult Index()
        {
            return View();
        }
       public IActionResult Test()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int writerId = Convert.ToInt32(userId);

            // Giriş yapan kullanıcının bilgilerini getiriyoruz.
            Writer writer = writerManager.TGetByID(writerId);

            // Giriş yapan kullanıcının son 3 blogunu getiriyoruz.
            var blogs = blogManager.GetRecentBlogsByWriter(writerId, 3);

            // ViewModel'e dolduruyoruz.
            ProfileViewModel viewModel = new ProfileViewModel
            {
                Writer = writer,
                Blogs = blogs
            };

            return View(viewModel);
        }
    }
}
