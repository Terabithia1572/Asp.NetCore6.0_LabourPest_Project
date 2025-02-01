using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogCategoryController : Controller
    {
        BlogCategoryManager blogCategoryManager = new BlogCategoryManager(new EfBlogCategoryRepository());
        public IActionResult CategoryList()
        {
            var values = blogCategoryManager.GetAll();
            return View(values);
        }
        public IActionResult BlogCategoryList()
        {
            var values = blogCategoryManager.GetAll();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddBlogCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddBlogCategory(BlogCategory blogCategory)
        {
            blogCategory.BlogCategoryStatus = true;
           blogCategoryManager.TAdd(blogCategory);
            return RedirectToAction("BlogCategoryList","AdminBlogCategory");
        }
        [HttpGet]
        public IActionResult UpdateBlogCategory(int id)
        {
            var values = blogCategoryManager.TGetByID(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateBlogCategory(BlogCategory blogCategory)
        {
            blogCategory.BlogCategoryStatus = true;
            blogCategoryManager.TUpdate(blogCategory);
            return RedirectToAction("BlogCategoryList", "AdminBlogCategory");
        }
        public IActionResult DeleteBlogCategory(int id)
        {
            var blogvalue = blogCategoryManager.TGetByID(id);
            blogCategoryManager.TDelete(blogvalue);
            return RedirectToAction("BlogCategoryList", "AdminBlogCategory");
        }
    }
}
