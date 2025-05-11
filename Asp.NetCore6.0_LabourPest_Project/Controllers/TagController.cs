using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
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
                .Include(bt => bt.Blogs).ThenInclude(b => b.Writer)
                .Include(bt => bt.Blogs).ThenInclude(b => b.BlogCategory)
                .Where(bt => bt.Tags.TagName == id)
                .ToList();

            // Sadece Blog listesini View'a göndermek için çıkarıyoruz
            var blogs = blogTags.Select(bt => bt.Blogs).ToList();

            ViewBag.Tag = id;
            return View(blogs);
        }
        [HttpGet]
        public IActionResult AddTag()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTag(Tag tag)
        {
            tag.TagName = tag.TagName.Trim().ToLower();
            if (!_context.Tags.Any(t => t.TagName == tag.TagName))
            {
                _context.Tags.Add(tag);
                _context.SaveChanges();
                ViewBag.Message = "Etiket başarıyla eklendi.";
            }
            else
            {
                ViewBag.Message = "Bu etiket zaten mevcut.";
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult TagList()
        {
            using var context = new Context();
            var tags = context.Tags.ToList();
            return View(tags);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTag(int id)
        {
            using var context = new Context();
            var tag = context.Tags.Find(id);
            if (tag != null)
            {
                context.Tags.Remove(tag);
                context.SaveChanges();
            }
            return RedirectToAction("TagList");
        }

    }
}
