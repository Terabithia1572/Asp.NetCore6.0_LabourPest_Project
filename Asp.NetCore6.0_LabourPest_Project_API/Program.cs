using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete; // Service sınıfını görmesi için eklendi

var builder = WebApplication.CreateBuilder(args);

// ✅ Veri Tabanı Context Tanımı
builder.Services.AddDbContext<Context>();

// ✅ API ve Swagger Desteği
// ✅ JSON Sonsuz Döngüsünü (Object Cycle) Engelleyen Güvenli Controller Tanımı
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ SENİN MİMARİNE UYGUN DOĞRU ENJEKSİYON (ÇÖZÜM BURASI KRAL!)
// ServiceManager constructor'da IGenericDal<Service> beklediği için bunu ekliyoruz:

builder.Services.AddScoped<IGenericDal<Services>, EfServicesRepository>();
builder.Services.AddScoped<IServicesDal, EfServicesRepository>(); // 🎯 İŞTE EKSİK OLAN SATIR BUYDU KRAL!
// Controller'da çağıracağımız IServiceService için de bunu ekliyoruz:
builder.Services.AddScoped<IServicesService, ServicesManager>();

// ✅ Blog Modülü İçin Bağımlılık Enjeksiyonları
builder.Services.AddScoped<IGenericDal<Blog>, EfBlogRepository>();
builder.Services.AddScoped<IBlogDal, EfBlogRepository>();
builder.Services.AddScoped<IBlogService, BlogManager>(); // Generic Service'ten türeyen ana arayüzün

// ✅ JobApplication Modülü İçin Bağımlılık Enjeksiyonu (Dependency Injection) Tanımı
builder.Services.AddScoped<IGenericDal<JobApplication>, EfJobApplicationRepository>();
builder.Services.AddScoped<IJobApplicationDal, EfJobApplicationRepository>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationManager>();

builder.Services.AddScoped<IGenericDal<FAQ>, EfFAQRepository>();
builder.Services.AddScoped<IFAQDal, EfFAQRepository>();
builder.Services.AddScoped<IFAQService, FAQManager>();

builder.Services.AddScoped<IGenericDal<Product>, EfProductRepository>();
builder.Services.AddScoped<IProductDal, EfProductRepository>();
builder.Services.AddScoped<IProductService, ProductManager>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();