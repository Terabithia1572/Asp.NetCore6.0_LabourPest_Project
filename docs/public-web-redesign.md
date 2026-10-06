# LabourPest public web yenilemesi — tam ana sayfa

> Önceki tasarım aşamasının tarihsel raporudur. Yorum izin listesi, hareketler, düğme renkleri ve Muğla haritası için [güncel iyileştirme raporunu](public-web-enhancements.md) esas alın. Boş yorum-ID izin listesi son kullanıcı talebiyle kaldırıldı.

4 Ekim 2026 tarihinde tamamlanan yerel sunum katmanı revizyonudur. Push, merge veya deployment yapılmadı. API, mobil uygulama, ortak iş mantığı, controller, authentication, yönetim/müşteri ekranı, veritabanı veya yapılandırma dosyası değiştirilmedi. Başlangıçtaki kullanıcı değişiklikleri korundu.

## Uygulanan sıra

1. Kompakt başlık ve gezinme.
2. Yönetim kayıtlarının tamamını kullanan ana görsel kaydırıcısı.
3. LabourPest Hakkında.
4. Hizmetler — en fazla 6 gerçek kayıt; 3/2/1 sütun.
5. Vizyon, misyon ve kalite politikası — önizleme ve tam metni açan üç panel.
6. Fotoğraf galerisi — ana sayfada 4 kayıt, tam galeri bağlantısı.
7. Mevcut İlaç Kategorileri — görsel ve açıklama; alışveriş işlevi eklenmedi.
8. Ayrı bülten abonelik bandı.
9. Son 3 blog yazısı.
10. Sıkça sorulan sorular.
11. Görünür yorum bırakma formu.
12. Müşteri yorumları kaydırıcısı / seçili kayıt yoksa sade boş durum.
13. Telefon, e-posta, açık adres ve mevcut iletişim formu bağlantısı.
14. Türkiye haritası, ofis ve hizmet bölgeleri.
15. Kompakt koyu yeşil altbilgi; mevcut sosyal, yasal ve Yunus İNAN bağlantıları.

Beyaz ve açık nötr yüzeyler, koyu yeşil başlıklar, turkuaz eylemler ve 1200 px içerik alanı kullanıldı. Ana görsel normal içerikte masaüstünde 480 px; telefon yüksekliği içeriğe göre değişir. Uzun başlıklar gerektiğinde alanı büyütür. Bölümler kart, metin paneli, görsel grid, yatay kategori, form ve iletişim bandı gibi farklı düzenler kullanır.

## Önemli dosyalar

Aşağıdaki yollar web projesine göredir: `Asp.NetCore6.0_LabourPest_Project/`.

- `Views/Shared/Public/_HomePageBody.cshtml`: kesin bölüm sırası.
- `Views/Shared/Components/PageSliderViewComponentPartial/Default.cshtml`: tüm dinamik slaytlar ve Türkçe erişilebilir kontroller.
- `wwwroot/public-web/js/carousels.js`: bağımlılıksız, yalnızca public sayfaya ait ana görsel ve yorum kaydırıcıları.
- `wwwroot/public-web/css/public.css`: .lp-public altında yalıtılmış responsive tasarım.
- `Views/Shared/Components/WhoWeUsViewComponentPartial/Default.cshtml`: tam kurumsal metinleri koruyan açılabilir paneller.
- `Views/Shared/Components/CommentViewComponentPartial/Default.cshtml` ve `wwwroot/public-web/js/review-form.js`: ana sayfadaki yorum formu ve mevcut hata dönüşünün görünür olması.
- `Presentation/PublicReviews.cs`, `ViewComponents/MainLayout/TestimonialsViewComponentPartial.cs` ve ilgili Razor view: yalnızca işletmenin seçtiği yorumları gösterecek sunum listesi.
- `Views/Shared/Public/_HomeContact.cshtml`, `Views/Shared/Components/LocationMapViewComponentPartial/Default.cshtml`: iletişim, ofis ve harita.
- `wwwroot/public-web/images/turkiye.svg` ve `turkiye-LICENSE.txt`: yerel harita ve lisansı.
- `Views/Home/Deneme.cshtml`, `Index.cshtml`, `Views/Shared/_PublicLayout.cshtml`, `Views/Shared/Public/_Footer.cshtml`: ortak sayfa gövdesi, betikler ve altbilgi.

Önceki yenilemenin public detay/liste sayfaları, menü, galeri, form ve güvenli görsel yardımcıları korundu. Yeni framework, paket veya bağımlılık yükseltmesi yoktur.

## Kaydırıcılar ve erişilebilirlik

Ana görsel kaydırıcısı önceki/sonraki düğmeler, seçili durumu bildirilen noktalar, Türkçe etiketler, klavye ve yatay dokunma hareketi ile çalışır. Otomatik döndürme yoktur. Bu nedenle bekleme, hover, odak veya reduced-motion durumunda kendiliğinden içerik değişmez. Düğmelere odaklanınca ok tuşları da kullanılabilir.

Yalnızca aktif slayt görünür ve içindeki bağlantılar normal sekme sırasına girer. Kaydırıcı kodu odağı başka yere taşımaz; dokunma isteği odaktaki bir bağlantıyı gizleyecekse değişim yapılmaz. İlk fotoğraf eager/high priority yüklenir; sonraki fotoğraflar ertelenir. İstenen görsel çözümlenene kadar önceki görsel görünür kalır; sonraki görsel önden hazırlanır. Hatalı görsel yerel fallback kullanır. Sıfır/tek kayıtta gereksiz kontroller yoktur.

Yorum kaydırıcısı en fazla 9 seçili kayıt gösterir; masaüstünde 3, tablette 2, telefonda 1 yorum görünür. Gerekmeyen kontroller gizlenir. Yorumlar Razor tarafından encode edilir; keyfi dış avatar URL'leri yüklenmez. Baş harf avatarları kullanılır; yıldız veya müşteri sayısı uydurulmadı.

Tek bir ana H1, native details/summary, benzersiz ID'ler, görünür focus, 44 px kaydırıcı kontrolleri, native galeri dialog'u, mobil menü Escape/odak/scroll davranışı ve sticky başlık altında görünür anchor konumu korundu. CAPTCHA hata dönüşünde mevcut mesajın bulunduğu alana odaklanılır; kullanıcı hata mesajını uzun sayfanın altında aramak zorunda kalmaz.

## Dinamik içerik, adresler ve form sözleşmeleri

Mevcut ViewComponent/manager veri kaynakları korundu. Slayt metinleri ve görselleri, hizmetler, kurumsal politikalar, galeri, kategoriler, blog ve SSS yönetim kayıtlarından gelir. Kurumsal metinler veri kaynağında değiştirilmedi; kısa önizlemenin yanında tamamı açılabilir.

Hizmet ve blog slug'ları, kategori görsel hedefleri, tam galeri ve mevcut liste/detay adresleri korunur. `Home/Deneme?bolum=...` ile hizmetler, kurumsal, blog, SSS, galeri, kategoriler, haşereler, ürünler ve yorum formu erişilebilir. Müşteri girişi `/Login/SignIN` olarak kalır.

Bülten: POST `/Subscribe/AddSubscribes`, alan `SubscribeMail`.
Yorum: POST `/MainComment/AddComment`, alanlar `CommentUserName`, `CommentTitle`, `CommentContent`, `ImageURL`; isteğe bağlı dosya alanı `file`.
Fotoğraf yükleme: mevcut `/MainComment/UploadImage`.
Antiforgery üretimi, reCAPTCHA site anahtarı, mevcut TempData hata mesajı, upload başarı/hata davranışı ve backend dönüşleri korunur. Fotoğraf zorunlu değildir. Sunucunun bildirmediği bir başarılı gönderim mesajı üretilmez. Mevcut sunucu işleme/validasyon altyapısı değiştirilmedi.

## Yorum seçimi — işletmeden beklenen içerik

`CommentStatus`, mevcut public ve yönetim ekleme/güncelleme kodlarında otomatik true yapıldığı için güvenilir yayın onayı değildir. Entity'de başka bir approved/featured alanı bulunmadı.

`Presentation/PublicReviews.cs` içindeki `SelectedIds` bilerek boştur. İşletmenin yayımlanmasını istediği kayıtların **CommentID** değerleri sağlanınca bu listeye, gösterim sırasıyla eklenebilir. En fazla 9 benzersiz pozitif kimlik alınır; mevcut manager ile yalnızca bu kayıtlar okunur. Liste boşken yorum tablosu sorgulanmaz. Rastgele kayıt seçimi, toplu GetAll veya status alanından yayın onayı çıkarımı yoktur.

Canlı sunumda şu anda boş durum gösterilir. Dolu kaydırıcı ekran görüntüleri yalnızca repository dışındaki geçici önizlemede üretilmiş ve her kartta “TEST İÇERİĞİ · Gerçek müşteri yorumu değildir” olarak etiketlenmiştir. Bu test kayıtları projede veya veritabanında bulunmaz.

## Harita ve doğrulanmış konumlar

Önce mevcut `canabicom/svg/turkiye-haritasi-final.svg` ve `final1.svg` incelendi. İl kodları eksik/yinelenmişti; tam ve doğru il eşlemesi için bu dosyalar değiştirilmeden bırakıldı. Yeni public harita aynı harita projesinin özgün kaynağından alındı:

- [dnomak/svg-turkiye-haritasi](https://github.com/dnomak/svg-turkiye-haritasi)
- [MIT lisansı](https://github.com/dnomak/svg-turkiye-haritasi/blob/master/LICENSE), Copyright 2015 Doğukan Güven Nomak.
- Kaynak deposunun belirttiği coğrafi kaynak: [Wikimedia Turkey provinces blank gray](https://commons.wikimedia.org/wiki/File:Turkey_provinces_blank_gray.svg).

Yerel SVG'de 01–81 il kodlarının tamamı doğrulandı. İl path geometrileri özgün kaynakla aynı tutuldu. Sadece renkler, açıklayıcı başlıklar ve Antalya etiketi eklendi; Türkiye dışındaki Kıbrıs grupları çıkarılıp görünüm alanı Türkiye'ye daraltıldı. Lisans dosyası asset ile birlikte bulunur. Harita SDK'sı, API anahtarı, uzak tile veya konum izni yoktur.

**Doğrulanan ofis:** Elmalı Mahallesi 7. Sokak, Zamanlar İş Merkezi No:18/406, Muratpaşa / Antalya. Telefon: +90 533 699 22 75. E-posta: murat.koc@labourpest.com. Kanıt mevcut LocationMap public bileşenindeki adres ve iletişim kaydıdır. Haritada yalnızca Antalya yeşildir; il adı, ofis etiketi ve metin listesi de görünürdür.

**Hizmet bölgeleri:** mevcut public harita metnindeki Ege ve Akdeniz korunur; il/ilçe uygunluğu için iletişim bağlantısı verilir. Bu bölgeler ofis olarak gösterilmez. Metadata'da geçen İzmir/Muğla ve Konya anahtar kelimesi yeni şube kanıtı sayılmadı; bu iller ayrıca işaretlenmedi. İl düzeyindeki ek şube/hizmet kapsamı için işletme doğrulaması gerekir.

## Doğrulama sonuçları

### Derleme ve gerçek uygulama

Son web derlemesi: `dotnet build ... --no-restore -v minimal` — **0 hata, 58 mevcut uyarı**. Uyarılar .NET 6 destek durumu, mevcut nullability ve legacy Razor kullanımlarına aittir. Bu revizyonun public dosyalarında yeni derleme uyarısı saptanmadı. İlk bağımlılık derlemesinde ortak projelerin uyarılarıyla toplam 204 uyarı görülmüştür; son web derleme günlüğü ayrı saklandı.

Gerçek uygulamada yalnızca GET kontrolleri yapıldı:

- İletişim, iş başvurusu, gizlilik ve müşteri girişi HTTP 200.
- İletişim/başvuru formlarında antiforgery alanı mevcut.
- Giriş ekranında public stil dosyaları yüklenmiyor.
- Veri gerektiren ana sayfa, yerel LabourPest veritabanı bulunmadığı için beklenen HTTP 500 sonucunu verdi. Test sürecinde SQL bağlantısı kapalı loopback porta yönlendirildi; üretime bağlantı yoktu. Bu override yalnızca süreç ortam değişkenindeydi.

### Geçici önizleme

Gerçek derlenmiş Razor dosyalarını kullanan, repository dışında geçici ASP.NET önizleme uygulaması kullanıldı. Önceki çalışmada alınmış public HTML fixture'ları yeniden kullanıldı; bu revizyonda üretim verisi okunmadı. Tüm gerçek POST işlemleri engellendi.

- 19 public görünüm, **360, 390, 768, 1024, 1440 ve 1920 CSS px** genişliklerinde kontrol edildi.
- 126 genel kontrol kaydı: HTTP, tek H1, benzersiz ID, taşma, yerel görseller, menü, galeri, SSS, form sözleşmeleri, JavaScript kapalı gezinme ve reflow; başarısız sonuç, console/JavaScript hatası veya eksik yerel asset isteği yok.
- 14 ana sayfa kontrol kaydı: kesin bölüm sırası, tüm slaytlar, klavye/odak, Chromium dokunma hareketi, otomatik hareket olmaması, boş/tek kayıt, uzun Türkçe metin, tam kurumsal açıklamalar, formlar, yorum sayfalama/encode, yavaş/hatalı görsel ve 81 ilin geometrisi; tamamı geçti.
- 10 ek kontrol kaydı: boş veri, sayfalama sınırları, blog yorumlarının encode edilmesi, CAPTCHA dönüşü, yalnızca taklit yanıtlarla upload başarı/hata davranışı; tamamı geçti.
- Son CAPTCHA hata mesajının odaklanması ve sticky başlığın altında görünmesi ayrıca geçti.
- Toplam **151 önizleme kontrol kaydı**. Gerçek uygulamanın 5 GET sonucu yukarıda ayrı listelenmiştir.
- %200 zoom için 1440 px ekranın 720 CSS px reflow karşılığı test edildi; bu gerçek zoom tuşlarıyla yapılan bir test değildir.
- Masaüstü/telefon ilk ekranları, tam ana sayfa, yorum formu, boş/dolu yorum alanı ve harita görsel olarak incelendi.

### Sınırlar

Gerçek veritabanı içeriğiyle ana sayfa, gerçek form teslimi, CAPTCHA doğrulaması, e-posta, dosya saklama veya oturum açma POST'u test edilmedi. Bunlar için üretime bağlanılmadı. Ekran görüntülerindeki reCAPTCHA alanı yalnızca geçici önizlemede boyutu gösteren, açıkça etiketli bir yer tutucudur; projeye eklenmedi. Fiziksel cihaz, ekran okuyucu ve Lighthouse ölçümü yapılmadı.

Mevcut yönetim metinlerindeki sağlık/garanti ifadeleri, eksik gizlilik metni ve metadata konum/çalışma saati tutarsızlıkları bu sunum revizyonunda yeniden yazılmadı. Yeni sertifika, müşteri sayısı, referans veya garanti iddiası üretilmedi.

## Korunan alanlar

Revizyon başında 1.839 mevcut dosyanın SHA-256 kaydı alındı. Son karşılaştırmada değişiklikler yalnızca belirtilen public sunum dosyaları ve bu raporla sınırlıdır. API, shared backend, migrations, controller'lar, configuration, administration/customer/authentication alanlarında bu revizyonun değişikliği yoktur. Flutter projesinin kendi Git çalışma ağacı temizdir; mobil dosyaya yazılmadı. Önceki kullanıcı değişiklikleri geri alınmadı.

## Ekran görüntüleri ve kanıt dosyaları

Çıktı klasörü:
`C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-2/`

- `home-first-1440.png`, `home-first-390.png`: ilk ekranlar.
- `home-full-1440.png`, `home-full-390.png`: son tam sayfa görüntüleri.
- `home-360.png`, `home-390.png`, `home-768.png`, `home-1024.png`, `home-1440.png`, `home-1920.png`: genişlik matrisi görüntüleri.
- `review-form-1440.png`, `review-form-390.png`: yorum formu.
- `reviews-empty-1440.png`, `reviews-empty-390.png`: gerçek başlangıç seçiminin boş hali.
- `review-carousel-test-1440.png`, `review-carousel-test-390.png`: açıkça etiketlenmiş geçici test kayıtlarıyla kaydırıcı.
- `map-1440.png`, `map-390.png`: harita/ofis bölümü.
- `browser-results.json`, `homepage-results.json`, `edge-results.json`, `feedback-result.json`: kontrol sonuçları.
- `final-build.log`, `scope-check.json`: derleme ve dosya koruma kanıtı.

Görüntüler yerel fixture önizlemesidir; güncel canlı veri doğrulaması değildir.
