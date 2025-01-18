using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminBlogCategoryController : Controller
    {
        public IActionResult CategoryList()
        {
            return View();
        }
    }
}
