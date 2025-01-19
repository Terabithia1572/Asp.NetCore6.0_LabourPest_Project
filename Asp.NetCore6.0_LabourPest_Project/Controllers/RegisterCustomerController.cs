using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	
    public class RegisterCustomerController : Controller
	{
        WriterManager writerManager = new WriterManager(new EfWriterRepository());
        public IActionResult Index()
		{
			return View();
		}
		public IActionResult CustomerList()
		{
			var values = writerManager.GetAll();
            return View(values);
		}
		[HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCustomer(Writer writer)
        {
            writer.WriterStatus = true;
            writerManager.TAdd(writer);
            return RedirectToAction("CustomerList", "RegisterCustomer");
        }
    }
}
