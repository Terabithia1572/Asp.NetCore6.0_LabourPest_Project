using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents.MainLayout
{
    public class ProductAlternatifViewComponentPartial:ViewComponent
    {
        ProductManager productManager = new(new EfProductRepository());
        public IViewComponentResult Invoke()
        {
            var values=productManager.GetListWithProductCategory()
                .OrderByDescending(x=>x.ProductID)
                .Take(6)
                .ToList();
            return View(values);
        }
    }
}
