using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class AlternativeProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
