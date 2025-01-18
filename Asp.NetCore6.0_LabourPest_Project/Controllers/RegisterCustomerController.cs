using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	public class RegisterCustomerController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult CustomerList()
		{
			return View();
		}
	}
}
