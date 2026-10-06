# Public web yorum regresyonları

Windows, .NET 6 çalışma zamanı/uygun SDK ve SQL Server LocalDB `MSSQLLocalDB` gerekir. Depo kökünde:

```powershell
dotnet test tests/PublicWeb.Tests/PublicWeb.Tests.csproj -c Release
```

Web uygulaması aynı build klasöründen çalışıyorsa kilitlenen DLL'leri önlemek için ayrı bir `-p:OutputPath=...` dizini kullanın.

Testler rastgele `LabourPest_PublicReviewTests_<guid>` adlı izole veritabanları oluşturur. Yalnız gerçek EF modelinden üretilen Comments tablosu kurulur; üretim veritabanı veya uygulamanın bağlantı ayarları okunmaz. `ConnectionStrings__DefaultConnection` yalnız test sürecinde geçici olarak atanır, önceki değer geri yüklenir. Test sonunda yalnız bu doğrulanmış isimli veritabanları silinir. Testleri paralel çalıştırmayın; test assembly'sinde paralellik kapalıdır.

36 test; eski MVC binding hatasını, gerçek `CommentManager → EfCommentRepository → Context.SaveChanges` akışını, fotoğraflı/fotoğrafsız kaydı, antiforgery reddini, doğrulama hatalarını, kayıt başarısızlığını, başarı sonrası GET'i, HTTP CAPTCHA yanıtlarını, public layout güvenlik başlıklarını ve değişen görsellerin orijinale dönmesini kapsar.

Yayınlanan eski Context, bağlantıyı kendi içinde tanımladığı için testlere özel `TestConnectionRedirect`, EF Core `ConnectionOpening` tanısını dinleyerek bağlantı açılmadan önce yalnız test veritabanına yönlendirir. Bu yardımcı yalnız test assembly'sinde bulunur ve uygulama/ortak Context kodunu değiştirmez. Yerel çalışma alanındaki farklı Context constructor veya configuration değişikliklerine bağımlılık yoktur.

CAPTCHA doğrulayıcı test dublörü yalnız test host'una kaydedilir. Gerçek HTTP doğrulayıcı testlerinde kontrollü HttpMessageHandler kullanılır. Bu testler Google'da insan doğrulaması yapmaz ve üretim için bir bypass sağlamaz. Fotoğraflı kayıt mevcut yükleme uç noktasının döndürdüğü yerel yol sözleşmesini kullanır; üretime dosya yüklenmez.
