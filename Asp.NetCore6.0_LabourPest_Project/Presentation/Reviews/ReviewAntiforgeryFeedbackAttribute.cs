using Asp.NetCore6._0_LabourPest_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Asp.NetCore6._0_LabourPest_Project.Presentation.Reviews;

// Keep the framework's antiforgery rejection, but render the existing public form.
public sealed class ReviewAntiforgeryFeedbackAttribute : Attribute, IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not IAntiforgeryValidationFailedResult) return;
        var input = new PublicReviewInput();
        if (context.HttpContext.Request.HasFormContentType)
        {
            try
            {
                var form = context.HttpContext.Request.Form;
                static string Read(string value, int maximum) => value[..Math.Min(value.Length, maximum)];
                input.CommentUserName = Read(form["CommentUserName"].ToString(), 100);
                input.CommentTitle = Read(form["CommentTitle"].ToString(), 200);
                input.CommentContent = Read(form["CommentContent"].ToString(), 5000);
            }
            catch (Exception error) when (error is InvalidDataException or BadHttpRequestException)
            {
                // An unreadable form may be why antiforgery rejected the request.
                // Keep the rejection; there are no safely recoverable fields.
            }
        }
        context.ModelState.AddModelError("", "Formun güvenlik doğrulaması geçersiz veya süresi dolmuş. Bilgilerinizi kontrol edip robot doğrulamasını yeniden tamamlayın.");
        var viewData = new ViewDataDictionary<PublicReviewInput>(new EmptyModelMetadataProvider(), context.ModelState) { Model = input };
        viewData["ReviewFailure"] = true;
        context.HttpContext.Response.Headers["Cache-Control"] = "no-store";
        context.Result = new ViewResult {
            ViewName = "~/Views/MainComment/AddComment.cshtml", ViewData = viewData, StatusCode = 400
        };
    }
    public void OnResultExecuted(ResultExecutedContext context) { }
}
