using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleErrorCode(int statusCode)
        {
            ViewData["ShowComponents"] = false;
            if (statusCode == 404)
            {
                // Orijinal isteğin URL bilgisini alıyoruz
                var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                var originalPath = statusCodeReExecuteFeature?.OriginalPath;

                // Eğer URL "/admin" ile başlıyorsa Admin Layout'u, aksi halde MainLayout'u kullanıyoruz
                if (!string.IsNullOrEmpty(originalPath) && originalPath.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                {
                    return View("NotFoundAdmin");
                }
                else
                {
                    return View("NotFoundMain");
                }
            }

            // Diğer hata kodları için varsayılan hata sayfası
            return View("Error");
        }
    }
}
