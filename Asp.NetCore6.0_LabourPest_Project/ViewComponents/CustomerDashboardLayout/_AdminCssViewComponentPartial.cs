using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.CustomerDashboardLayout
{
    public class _AdminCssViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
