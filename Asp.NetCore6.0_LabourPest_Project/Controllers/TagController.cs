using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class TagController : Controller
    {
        private readonly Context _context;

        public TagController(Context context)
        {
            _context = context;
        }

        public IActionResult Blogs(string id) // id = TagName
        {
            // BlogTag'ler üzerinden Include ile Blog -> Writer ve BlogCategory bilgilerini çekiyoruz
            var blogTags = _context.BlogTags
                .Include(bt => bt.Blog).ThenInclude(b => b.Writer)
                .Include(bt => bt.Blog).ThenInclude(b => b.BlogCategory)
                .Where(bt => bt.Tag.TagName == id)
                .ToList();

            // Sadece Blog listesini View'a göndermek için çıkarıyoruz
            var blogs = blogTags.Select(bt => bt.Blog).ToList();

            ViewBag.Tag = id;
            return View(blogs);
        }
    }
}
