using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================================
// ✅ VERİ TABANI CONTEXT TANIMI
// =====================================================================================
builder.Services.AddDbContext<Context>();

// =====================================================================================
// ✅ API VE SWAGGER DESTEĞİ + JSON DÖNGÜSÜ (OBJECT CYCLE) KORUMASI
// =====================================================================================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================================================
// ✅ BAĞIMLILIK ENJEKSİYONLARI (DEPENDENCY INJECTION)
// =====================================================================================

// 1. Services Modülü
builder.Services.AddScoped<IGenericDal<Services>, EfServicesRepository>();
builder.Services.AddScoped<IServicesDal, EfServicesRepository>();
builder.Services.AddScoped<IServicesService, ServicesManager>();

// 2. Blog Modülü
builder.Services.AddScoped<IGenericDal<Blog>, EfBlogRepository>();
builder.Services.AddScoped<IBlogDal, EfBlogRepository>();
builder.Services.AddScoped<IBlogService, BlogManager>();

// 3. JobApplication Modülü
builder.Services.AddScoped<IGenericDal<JobApplication>, EfJobApplicationRepository>();
builder.Services.AddScoped<IJobApplicationDal, EfJobApplicationRepository>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationManager>();

// 4. FAQ Modülü
builder.Services.AddScoped<IGenericDal<FAQ>, EfFAQRepository>();
builder.Services.AddScoped<IFAQDal, EfFAQRepository>();
builder.Services.AddScoped<IFAQService, FAQManager>();

// 5. Product Modülü
builder.Services.AddScoped<IGenericDal<Product>, EfProductRepository>();
builder.Services.AddScoped<IProductDal, EfProductRepository>();
builder.Services.AddScoped<IProductService, ProductManager>();

// 🎯 EKSİK OLAN VE YENİ EKLENEN MODÜLLER BURADAN BAŞLIYOR, KRAL!

// 6. Admins Modülü (Yönetici Giriş ve Yetki)
builder.Services.AddScoped<IGenericDal<Admin>, EfAdminRepository>();
builder.Services.AddScoped<IAdminDal, EfAdminRepository>();
builder.Services.AddScoped<IAdminService, AdminManager>();

// 7. Categories Modülü (Ürün Kategorileri)
builder.Services.AddScoped<IGenericDal<Category>, EfCategoryRepository>();
builder.Services.AddScoped<ICategoryDal, EfCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

// 8. BlogCategories Modülü (Blog Yazı Kategorileri)
builder.Services.AddScoped<IGenericDal<BlogCategory>, EfBlogCategoryRepository>();
builder.Services.AddScoped<IBlogCategoryDal, EfBlogCategoryRepository>();
builder.Services.AddScoped<IBlogCategoryService, BlogCategoryManager>();

// 9. Brands Modülü (Referans Markalar)
builder.Services.AddScoped<IGenericDal<Brands>, EfBrandsRepository>();
builder.Services.AddScoped<IBrandsDal, EfBrandsRepository>();
builder.Services.AddScoped<IBrandsService, BrandsManager>();

// 10. Employees Modülü (Çalışanlarımız/Uzman Kadro)
builder.Services.AddScoped<IGenericDal<Employee>, EfEmployeeRepository>();
builder.Services.AddScoped<IEmployeeDal, EfEmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeManager>();

// 11. HomePages Modülü (Ana Sayfa Slider/Karşılama İçerikleri)
builder.Services.AddScoped<IGenericDal<HomePage>, EfHomePageRepository>();
builder.Services.AddScoped<IHomePageDal, EfHomePageRepository>();
builder.Services.AddScoped<IHomePageService, HomePageManager>();

// 12. Comments Modülü (Müşteri/Referans Yorumları)
builder.Services.AddScoped<IGenericDal<Comment>, EfCommentRepository>();
builder.Services.AddScoped<ICommentDal, EfCommentRepository>();
builder.Services.AddScoped<ICommentService, CommentManager>();

// 13. Subscribes Modülü (E-Bülten Aboneliği)
builder.Services.AddScoped<IGenericDal<Subscribe>, EfSubscribeRepository>();
builder.Services.AddScoped<ISubscribeDal, EfSubscribeRepository>();
builder.Services.AddScoped<ISubscribeService, SubscribeManager>();

// 14. Writers Modülü (Blog Yazarları)
builder.Services.AddScoped<IGenericDal<Writer>, EfWriterRepository>();
builder.Services.AddScoped<IWriterDal, EfWriterRepository>();
builder.Services.AddScoped<IWriterService, WriterManager>();

// 15. Contacts Modülü (İletişim Formları ve Mesajlar)
builder.Services.AddScoped<IGenericDal<Contact>, EfContactRepository>();
builder.Services.AddScoped<IContactDal, EfContactRepository>();
builder.Services.AddScoped<IContactService, ContactManager>();

// 16. WhoWeUs Modülü (Biz Kimiz? )
builder.Services.AddScoped<IGenericDal<WhoWeUs>, EfWhoWeUsRepository>();
builder.Services.AddScoped<IWhoWeUsDal, EfWhoWeUsRepository>();
builder.Services.AddScoped<IWhoWeUsService, WhoWeUsManager>();

// 17. BlogComments Modülü İçin Bağımlılık Enjeksiyonları
builder.Services.AddScoped<IGenericDal<BlogComment>, EfBlogCommentRepository>();
builder.Services.AddScoped<IBlogCommentDal, EfBlogCommentRepository>();
builder.Services.AddScoped<IBlogCommentService, BlogCommentManager>();

// =====================================================================================
// ✅ APPLICATION PIPELINE (HTTP İSTEK KANALI)
// =====================================================================================
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