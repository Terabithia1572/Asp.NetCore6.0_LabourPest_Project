using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminBlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BlogList()
        {
            return View();
        }
    }
}
