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

    // Güvenlik için proxy IP adreslerini burada tanýmlayýn. Örneðin:
    // options.KnownProxies.Add(IPAddress.Parse("PROXY_IP_ADDRESS"));
});
// Add services to the container.
builder.Services.AddDbContext<Context>(); // BU SATIRI EKLE
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // <- Bunu ekle
builder.Services.AddScoped<IBlogDal, EfBlogRepository>(); // IBlogRepository için EfBlogRepository kullanýmý
builder.Services.AddScoped<BlogManager>(); // BlogManager'ý ekleyin
builder.Services.AddControllersWithViews(options =>
{
    // Tüm isteklerde kullanýcý doðrulamasý gerektiren global yetkilendirme filtresi ekliyoruz.
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/SignIN";
    });


builder.Services.AddSession();
var app = builder.Build();
app.UseStatusCodePagesWithReExecute("/Error/{0}");



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Deneme");
    app.UseHsts();
}

// Controllers ve MVC yapýlandýrmasýný ekliyoruz. 




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
        name: "default",
        pattern: "{controller=Home}/{action=Deneme}/{id?}");
    endpoints.MapControllers(); // UploadImage için gerekli
});
app.MapHub<NotificationHub>("/notificationhub");


app.Run();
