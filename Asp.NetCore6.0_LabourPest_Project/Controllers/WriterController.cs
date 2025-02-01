using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class WriterController : Controller
    {
        Context context = new Context();
        public IActionResult Index()
        {
            return View();
        }
       public IActionResult Test()
        {
            return View();
        }
    }
}
