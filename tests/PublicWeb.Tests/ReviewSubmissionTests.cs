using System.Net;
using System.Text.RegularExpressions;
using Asp.NetCore6._0_LabourPest_Project.Controllers;
using Asp.NetCore6._0_LabourPest_Project.Models;
using Asp.NetCore6._0_LabourPest_Project.Presentation.Reviews;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace PublicWeb.Tests;

public sealed class ReviewSubmissionTests : IClassFixture<ReviewDatabase>, IDisposable
{
    private readonly ReviewDatabase database;
    private readonly TestServer server;
    private readonly HttpClient client;
    private readonly ControlledVerifier verifier = new();
    private string cookie = "";
    private string token = "";

    public ReviewSubmissionTests(ReviewDatabase database)
    {
        this.database = database;
        database.Clear();
        server = new TestServer(new WebHostBuilder().UseEnvironment("Development")
            .ConfigureServices(services => {
                services.AddLogging();
                services.AddDataProtection().UseEphemeralDataProtectionProvider();
                services.AddControllersWithViews().AddApplicationPart(typeof(MainCommentController).Assembly);
                services.AddHttpContextAccessor();
                services.AddSingleton<Asp.NetCore6._0_LabourPest_Project.Presentation.PublicImageVariants>();
                services.AddOptions<RecaptchaOptions>();
                services.AddScoped<ICommentDal, EfCommentRepository>();
                services.AddScoped<ICommentService, CommentManager>();
                services.AddSingleton<IReviewCaptchaVerifier>(verifier);
            }).Configure(app => {
                // Exercise the real public review GET after redirect without requiring the
                // unrelated content tables; full homepage rendering has separate browser checks.
                app.Use(async (context, next) => {
                    // Reproduce the published host's restrictive headers before Razor.
                    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline';";
                    context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                    if (context.Request.Method == "GET" && context.Request.Path == "/")
                        context.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString("?bolum=yorum");
                    await next();
                });
                app.UseRouting(); app.UseAuthorization();
                app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Home}/{action=Deneme}/{id?}"));
            }));
        client = server.CreateClient();
    }

    [Theory]
    [InlineData("")]
    [InlineData("/canabicom/profilePhoto/test-photo.webp")]
    public async Task Valid_review_persists_once_then_redirects_to_GET_with_server_owned_fields(string image)
    {
        var response = await Post(new() { ["ImageUrl"] = image, ["CommentID"] = "987", ["CommentStatus"] = "false", ["CommentDate"] = "2001-01-01" });
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.EndsWith("#comment", response.Headers.Location!.OriginalString);
        Assert.Equal(1, database.Count);
        using var context = new Context();
        var row = Assert.Single(context.Comments.ToList());
        Assert.Equal(image.Length == 0 ? PublicReviewInput.DefaultAvatar : image, row.ImageUrl);
        Assert.NotEqual(987, row.CommentID);
        Assert.True(row.CommentStatus);
        Assert.Equal(DateTime.Today, row.CommentDate);
        CaptureCookies(response);
        var get = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
        get.Headers.Add("Cookie", cookie);
        var html = WebUtility.HtmlDecode(await (await client.SendAsync(get)).Content.ReadAsStringAsync());
        Assert.Contains("Yorumunuz alındı", html);
        await client.GetAsync(response.Headers.Location);
        Assert.Equal(1, database.Count);
        Assert.Equal(1, verifier.Calls);
    }

    [Fact]
    public async Task Public_layout_permits_CAPTCHA_without_unrelated_host_policy_changes()
    {
        var response = await client.GetAsync("/Home/Deneme?bolum=yorum");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var policy = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));
        Assert.Contains("https://www.google.com/recaptcha/", policy);
        Assert.Contains("https://www.gstatic.com/recaptcha/", policy);
        Assert.Contains("form-action 'self'", policy);
        Assert.False(response.Headers.Contains("Cross-Origin-Embedder-Policy"));
        Assert.Equal("same-origin-allow-popups", Assert.Single(response.Headers.GetValues("Cross-Origin-Opener-Policy")));
    }

    [Theory]
    [InlineData("CommentUserName", "")]
    [InlineData("CommentTitle", "   ")]
    [InlineData("CommentContent", "")]
    [InlineData("ImageUrl", "https://invalid.example/avatar.jpg")]
    public async Task Invalid_fields_keep_encoded_input_without_persistence(string field, string value)
    {
        var response = await Post(new() { ["CommentUserName"] = "<script>test</script>", [field] = value });
        Assert.Equal(422, (int)response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("lp-public", html);
        Assert.DoesNotContain("<script>test</script>", html);
        if (field != "CommentContent") Assert.Contains("Gerçek müşteri yorumu değildir.", WebUtility.HtmlDecode(html));
        Assert.Equal(0, database.Count);
        Assert.Equal(0, verifier.Calls);
        Assert.DoesNotContain("Gerçek müşteri", cookie);
    }

    [Theory]
    [InlineData("CommentUserName", 101)]
    [InlineData("CommentTitle", 201)]
    [InlineData("CommentContent", 5001)]
    public async Task Overlong_fields_do_not_persist(string field, int length)
    {
        Assert.Equal(422, (int)(await Post(new() { [field] = new string('x', length) })).StatusCode);
        Assert.Equal(0, database.Count);
    }

    [Theory]
    [InlineData(CaptchaVerification.Missing, "", 422)]
    [InlineData(CaptchaVerification.Rejected, "rejected-test-token", 422)]
    [InlineData(CaptchaVerification.Expired, "expired-test-token", 422)]
    [InlineData(CaptchaVerification.Unavailable, "timeout-test-token", 503)]
    [InlineData(CaptchaVerification.NotConfigured, "test-token", 503)]
    public async Task CAPTCHA_failure_preserves_form_but_never_persists(CaptchaVerification outcome, string value, int status)
    {
        verifier.Outcome = outcome;
        var response = await Post(new() { ["g-recaptcha-response"] = value });
        Assert.Equal(status, (int)response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("Gerçek müşteri yorumu değildir.", html);
        Assert.Contains("Yerel test", html);
        Assert.DoesNotContain("Yorumunuz alındı", html);
        Assert.Equal(0, database.Count);
    }

    [Fact]
    public async Task Missing_antiforgery_is_rejected_before_CAPTCHA_and_persistence()
    {
        var response = await Post(new() { ["__RequestVerificationToken"] = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("lp-public", html);
        Assert.Contains("güvenlik doğrulaması", html);
        Assert.Contains("Gerçek müşteri yorumu değildir.", html);
        Assert.Equal(0, verifier.Calls);
        Assert.Equal(0, database.Count);
    }

    [Fact]
    public async Task Malformed_form_keeps_antiforgery_rejection_without_a_generic_error()
    {
        using var content = new StringContent("unreadable-form");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
        var response = await client.PostAsync("/MainComment/AddComment", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("lp-public", html);
        Assert.Contains("güvenlik doğrulaması", html);
        Assert.Equal(0, verifier.Calls);
        Assert.Equal(0, database.Count);
    }

    [Fact]
    public async Task Database_failure_does_not_redirect_or_report_success()
    {
        ReviewDatabase.Execute(database.Connection, "CREATE TRIGGER RejectTestComment ON dbo.Comments INSTEAD OF INSERT AS THROW 50000, 'Controlled test failure', 1;");
        try
        {
            var response = await Post(new());
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            Assert.Contains("kaydedildiğini doğrulayamadık", html);
            Assert.DoesNotContain("Yorumunuz alındı", html);
            Assert.Null(response.Headers.Location);
            Assert.Equal(0, database.Count);
        }
        finally { ReviewDatabase.Execute(database.Connection, "DROP TRIGGER dbo.RejectTestComment"); }
    }

    private async Task<HttpResponseMessage> Post(Dictionary<string, string> changes)
    {
        var page = await client.GetAsync("/Home/Deneme?bolum=yorum");
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        CaptureCookies(page);
        var html = await page.Content.ReadAsStringAsync();
        var input = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*>").Value;
        token = WebUtility.HtmlDecode(Regex.Match(input, "value=\"([^\"]+)\"").Groups[1].Value);
        Assert.NotEmpty(token);
        var values = new Dictionary<string, string> {
            ["CommentUserName"] = "Yerel test", ["CommentTitle"] = "Yalnız test",
            ["CommentContent"] = "Gerçek müşteri yorumu değildir.", ["ImageUrl"] = "",
            ["g-recaptcha-response"] = "controlled-test-token", ["__RequestVerificationToken"] = token
        };
        foreach (var change in changes) values[change.Key] = change.Value;
        var request = new HttpRequestMessage(HttpMethod.Post, "/MainComment/AddComment") { Content = new FormUrlEncodedContent(values) };
        request.Headers.Add("Cookie", cookie);
        return await client.SendAsync(request);
    }

    private void CaptureCookies(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Set-Cookie", out var values))
            cookie = string.Join("; ", new[] { cookie }.Concat(values.Select(x => x.Split(';')[0])).Where(x => x.Length > 0));
    }
    public void Dispose() { client.Dispose(); server.Dispose(); }
    private sealed class ControlledVerifier : IReviewCaptchaVerifier
    {
        public CaptchaVerification Outcome { get; set; } = CaptchaVerification.Valid;
        public int Calls { get; private set; }
        public Task<CaptchaVerification> VerifyAsync(string? token, CancellationToken cancellationToken) { Calls++; return Task.FromResult(Outcome); }
    }
}
