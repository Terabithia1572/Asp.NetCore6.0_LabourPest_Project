using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ProfileSettings()
        {
            return View();
        }
    }
}
