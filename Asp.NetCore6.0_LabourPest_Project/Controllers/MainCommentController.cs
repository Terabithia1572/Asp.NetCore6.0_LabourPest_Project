using Asp.NetCore6._0_LabourPest_Project.Models;
using Asp.NetCore6._0_LabourPest_Project.Presentation.Reviews;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    [AllowAnonymous]
    public class MainCommentController : Controller
    {
        private readonly BusinessLayer.Abstract.ICommentService comments;
        private readonly IReviewCaptchaVerifier captcha;
        private readonly ILogger<MainCommentController> logger;

        public MainCommentController(BusinessLayer.Abstract.ICommentService comments, IReviewCaptchaVerifier captcha, ILogger<MainCommentController> logger)
        {
            this.comments = comments;
            this.captcha = captcha;
            this.logger = logger;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReviewAntiforgeryFeedback]
        [RequestSizeLimit(65536)]
        public async Task<IActionResult> AddComment(PublicReviewInput input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return ReviewFailure(input, 422);

            var verification = await captcha.VerifyAsync(Request.Form["g-recaptcha-response"].ToString(), cancellationToken);
            if (verification != CaptchaVerification.Valid)
            {
                var message = verification switch {
                    CaptchaVerification.Missing => "Lütfen 'Ben robot değilim' doğrulamasını tamamlayın.",
                    CaptchaVerification.Expired => "Doğrulamanın süresi doldu veya daha önce kullanıldı. Lütfen yeniden doğrulayın.",
                    CaptchaVerification.Rejected => "Doğrulama kabul edilmedi. Lütfen yeniden doğrulayın.",
                    _ => "Doğrulama hizmetine şu anda ulaşılamıyor. Bilgileriniz korunuyor; lütfen biraz sonra yeniden deneyin."
                };
                ModelState.AddModelError("", message);
                return ReviewFailure(input, verification is CaptchaVerification.Unavailable or CaptchaVerification.NotConfigured ? 503 : 422);
            }

            var comment = new Comment {
                CommentUserName = input.CommentUserName!.Trim(),
                CommentTitle = input.CommentTitle!.Trim(),
                CommentContent = input.CommentContent!.Trim(),
                // The form photo is optional, but the existing ImageUrl column is NOT NULL.
                ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? PublicReviewInput.DefaultAvatar : input.ImageUrl,
                CommentDate = DateTime.Today,
                CommentStatus = true
            };
            try
            {
                comments.TAdd(comment);
            }
            catch (Exception exception)
            {
                var sql = exception.GetBaseException() as Microsoft.Data.SqlClient.SqlException;
                logger.LogError("Public review persistence failed. Type={ExceptionType}; SqlNumber={SqlNumber}; Trace={Trace}; Location={Location}",
                    exception.GetType().Name, sql?.Number, HttpContext.TraceIdentifier, exception.StackTrace);
                ModelState.AddModelError("", "Kaydetme sırasında sorun oluştu. Yorumunuzun kaydedildiğini doğrulayamadık. Tekrar göndermeden önce bizimle iletişime geçebilirsiniz.");
                return ReviewFailure(input, 503);
            }

            // Success is set only after SaveChanges returns. Refresh now performs a GET.
            TempData["ReviewSuccess"] = "Yorumunuz alındı. Deneyiminizi paylaştığınız için teşekkür ederiz.";
            return RedirectToAction("Deneme", "Home", null, "comment");
        }

        private IActionResult ReviewFailure(PublicReviewInput input, int status)
        {
            Response.StatusCode = status;
            Response.Headers["Cache-Control"] = "no-store";
            ViewData["ReviewFailure"] = true;
            return View("AddComment", input);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Klasör yolunu tanımla
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "canabicom", "profilePhoto");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Dosya ismini oluştur
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Dosya yolunu oluştur
                string filePath = Path.Combine(folderPath, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Dosya yolunu döndür
                string relativePath = $"/canabicom/profilePhoto/{fileName}";
                return Json(new { filePath = relativePath });
            }

            return BadRequest("Dosya yüklenemedi!");
        }
    }
}
