using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogController : Controller
    {
        LogManager logManager = new LogManager(new EfLogRepository());
        public IActionResult LogList()
        {
            var values = logManager.GetAll()
                         .OrderByDescending(x => x.Id) // Listeyi Id'ye göre tersten sırala (en son eklenen en üstte olacak)
                         .Take(10)                      // Son 10 öğeyi al
                         .ToList();
            return View(values);
        }
        public IActionResult LogListAll()
        {
            var values = logManager.GetAll();

            return View(values);
        }
    }
}
