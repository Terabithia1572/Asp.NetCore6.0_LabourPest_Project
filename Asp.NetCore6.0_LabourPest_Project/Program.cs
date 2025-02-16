using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // G�venlik i�in proxy IP adreslerini burada tan�mlay�n. �rne�in:
    // options.KnownProxies.Add(IPAddress.Parse("PROXY_IP_ADDRESS"));
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBlogDal, EfBlogRepository>(); // IBlogRepository i�in EfBlogRepository kullan�m�
builder.Services.AddScoped<BlogManager>(); // BlogManager'� ekleyin
builder.Services.AddControllersWithViews(options =>
{
    // T�m isteklerde kullan�c� do�rulamas� gerektiren global yetkilendirme filtresi ekliyoruz.
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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Controllers ve MVC yap�land�rmas�n� ekliyoruz. 




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
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers(); // UploadImage i�in gerekli
});


app.Run();
