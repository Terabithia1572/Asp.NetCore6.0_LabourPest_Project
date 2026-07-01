using Asp.NetCore6._0_LabourPest_Project.Hubs;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddDbContext<Context>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddScoped<IBlogDal, EfBlogRepository>();
builder.Services.AddScoped<BlogManager>();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/SignIN";
    });

builder.Services.AddSession();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

// ✅ Güvenlik başlıkları ekleniyor
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:;");

    context.Response.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");

    context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");

    // ✅ Yeni Eklenen Güvenlik Header'ları
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), camera=(), microphone=()");
    context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");

    await next();
});


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Deneme");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "blogdetails",
        pattern: "blogdetails/{slug}",
        defaults: new { controller = "Blog", action = "BlogDetails" }
    );

    endpoints.MapControllerRoute(
        name: "servicedetails",
        pattern: "services/detail/{slug}",
        defaults: new { controller = "Services", action = "Detail" }
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Deneme}/{id?}"
    );

    endpoints.MapControllers();
});

app.MapHub<NotificationHub>("/notificationhub");

app.Run();
