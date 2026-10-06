# LabourPest — mevcut tasarımın iyileştirilmesi

4 Ekim 2026. Onaylanan tasarımın bölüm sırası, metinleri, görselleri, kart düzenleri ve responsive yapısı korunarak yerel iyileştirme tamamlandı. Değişiklikler commit, push, merge veya deployment yapılmadan çalışma ağacında bırakıldı.

## Yapılanlar

- Ana kaydırıcıya 760 ms görsel geçişi ve küçük dikey hareketli metin geçişi eklendi. Yeni görsel hazır olana kadar eski slayt görünür kalır; geçiş bitince eski slayt gizlenir. Pasif slaytlar `inert`, `aria-hidden` ve pointer engeliyle etkileşimden çıkarılır. Hızlı tıklamalar, iptal edilen animasyonlar ve bekleyen görsel yüklemesinden başka slayta geçiş desteklenir. Otomatik oynatma eklenmedi; klavye odağı taşınmaz.
- Hizmet ve blog görselleri, ilgili detay eylemiyle aynı `Url.RouteUrl` sonucunu kullanan gerçek bağlantılar oldu. Eksik hizmet slug'ında geçersiz bağlantı üretilmez.
- Hizmet/blog kartlarına 5 px, kategori/yorum kartlarına 3 px hareket; görsellere ölçülü yakınlaşma, gölge ve sınır geri bildirimi eklendi. Galerinin mevcut görüntüleyicisi, Escape ve odak dönüşü korundu. Hareket yalnız uygun ince işaretçi/hover ortamında uygulanır; klavye odağı da görünür geri bildirim alır.
- SSS, ölçülen gerçek yükseklik ve opaklıkla 320 ms içinde açılıp kapanır. Kapanış tamamlanana kadar `open` korunur; ardından panel doğal yüksekliğe döner. Hızlı tersine çevirme, içerik/ekran boyutu değişimi ve JavaScript olmadan native `details` davranışı çalışır.
- Yorum kaydırıcısı 480 ms geçiş, masaüstü/tablet/telefon için 3/2/1 kart, boş/tek/az kayıt ve uzun metin desteği kullanır. Uzun yorumlar kesilmez. Sahte puan/yıldız eklenmedi.
- Ortak süreler: kontrol 180, kart 300, SSS 320, ana kaydırıcı 760, yorum 480, bölüm başlığı 500 ms. Eğri `cubic-bezier(.22,1,.36,1)`. Sayfa açılışında ekran altında kalan uygun başlıklara yalnız bir kez 12 px giriş hareketi uygulanır; içerik varsayılan olarak görünürdür. Azaltılmış hareket tercihinde dekoratif hareketler kaldırılır ve durum değişimleri hemen tamamlanır. Büyük animasyon kütüphanesi eklenmedi.

Ana sayfanın 15 bölümlük sırası aynı kaldı. Diğer içerik bölümlerinin ölçülen yükseklikleri altı ekran genişliğinde başlangıçla 1 px tolerans içinde aynı. Gerekli küçük istisnalar: telefon ana kaydırıcısı bütün slaytlardaki en yüksek doğal içeriğe sabitlendi; harita iletişim bilgileri kadar uzadı; iletişim adresi istenen üç satıra alındı. Görseller ve pazarlama metinleri değiştirilmedi.

## Yorumların boş görünmesinin nedeni ve düzeltme

Sunum katmanındaki `PublicReviews.SelectedIds` boştu. Önceki talebe göre oluşturulmuş bu izin listesi, tabloda kayıt bulunsa bile yorum bileşeninin boş model döndürmesine yol açıyordu. Son talebe uygun olarak liste ve ona bağlı eleme kaldırıldı.

Doğru mevcut yol:

`TestimonialsViewComponentPartial → CommentManager.GetAll() → EfCommentRepository / GenericRepository<Comment>.GetListAll() → Context.Comments → Comment`

`MainComment`, yorum formunu işleyen controller adıdır; ayrı bir müşteri yorumu tablosu değildir. Blog yorumları ayrı `BlogComment` / `BlogsComments` yolunu kullanır. Razor modeli mevcut `Comment` kayıtlarını gösterir; ayrıca boş izin listesi uygulamaz.

Bileşen mevcut salt okunur servis çağrısını kullanır; kayıtları tarih, ardından ID azalan sıralayıp mevcut sunum sınırı olan en yeni 9 kayıtla sınırlar. ID sabitlemesi yoktur. Ortak servis/repository davranışı değiştirilmedi.

`CommentStatus`, gönderim sırasında otomatik true atandığından yönetici onayı olarak yorumlanmadı. İncelenen mevcut repository yolunda yayın/görünürlük filtresi veya EF global sorgu filtresi yok; silme işlemi kaydı kaldırıyor. Yeni status filtresi ya da moderasyon akışı eklenmedi. Kullanıcı metinleri Razor tarafından encode edilir; avatar olarak baş harfler kullanılır, harici profil URL'leri yüklenmez.

**Gerçek veri doğrulama sınırı:** Yerel LocalDB örneğinde LabourPest veritabanı bulunamadı; üretim veritabanına bağlanılmadı. Gerçek bileşen ve gerçek `CommentManager`, geçici test repository'siyle çalıştırıldı: tek `GetAll` okuması, en yeni 9 kaydın doğru sırası ve status üzerinden yeni eleme yapılmadığı doğrulandı. Ekran/kaydırıcı testleri açıkça `TEST` etiketli geçici yorumlarla yapıldı. Canlı/üretim yorumlarının alındığı iddia edilmiyor; test verileri depoya veya veritabanına eklenmedi.

Önceden bildirilen ilgisiz/spam kayıtlar, mevcut okuma kurallarıyla dönebilir. Bunlar ayrı içerik temizliği konusudur. Dil üzerinden sınıflandırma, kayıt silme/değiştirme veya tüm yorumları yeniden gizleme yapılmadı.

## İki düğmenin gerçek renkleri ve kontrastı

“Abone Ol” ve yorum gönderme düğmeleri tarayıcıda hesaplanan renklerle kontrol edildi. Önceki arka plan `#55c2bb` üzerinde beyazın kontrastı yalnız **2,14:1**; önceki hover arka planı `#70d3ca` üzerinde **1,77:1** idi. Bu nedenle aynı turkuaz tonu koyulaştırıldı. Değişiklik yalnız bu iki düğmeye uygulandı.

Etiket ve varsa iç ikon bütün durumlarda `#ffffff`:

- Normal ve klavye odağı: arka plan `#2e817c`, kontrast **4,62:1**.
- Hover: `#2a7873`, **5,21:1**.
- Basılı: `#256c68`, **6,13:1**.
- Devre dışı: `#547875`, **4,86:1**; metni solduracak opacity uygulanmaz.

Tüm değerler normal boy metin için 4,5:1 eşiğini geçer. Form action/method, binding, antiforgery, CAPTCHA, yükleme uçları ve gönderim altyapısı korundu.

## Harita ve iletişim

Mevcut SVG geometrisi korundu. `antalya`, plaka `07`, ve `mugla`, plaka `48`, aynı **`#237e70`** dolgusunu kullanıyor. Diğer 79 il **`#d8e1dc`**. Görünür etiket, alternatif metin, açıklama ve hizmet bölgesi listesi Antalya ve Muğla ile tutarlı. Muğla için fiziksel ofis uydurulmadı; adres açıkça Antalya ofisine ait.

Harita yanında şu bilgiler gösterilir:

Elmalı Mahallesi 7.Sokak
Zamanlar İş Merkezi No:18/406
Muratpaşa/ANTALYA

Telefon: `+90 533 699 22 75` — `tel:+905336992275`

E-posta: `murat.koc@labourpest.com`

Gerçek DOM bağlantısı tam olarak:

`mailto:murat.koc@labourpest.com?subject=%C4%B0la%C3%A7lama%20Talebi&body=Merhaba,%0A%0A%C4%B0la%C3%A7lama%20hizmeti%20hakk%C4%B1nda%20bilgi%20almak%20istiyorum...`

HTML kaynakta sorgu ayırıcı `&amp;` ile doğru encode edilir. İletişim ve altbilgideki adres/telefon yazımı aynı bilgilerle uyumlu hale getirildi; bölüm düzenleri korundu. Telefon/e-posta bağlantıları okunarak doğrulandı, arama veya mesaj başlatılmadı.

## Önemli dosyalar

Aşağıdaki yollar `Asp.NetCore6.0_LabourPest_Project/` altındadır:

- `wwwroot/public-web/js/motion.js`: yeni ortak animasyon yardımcısı, SSS ve tek seferlik başlık hareketleri.
- `wwwroot/public-web/js/carousels.js`: ana ve yorum kaydırıcılarının geçişi, yükleme/iptal/erişilebilirlik/resize davranışı.
- `wwwroot/public-web/css/public.css`: hareket değişkenleri, kart geri bildirimi, iki düğmenin kontrastı ve harita iletişim düzeni.
- `Views/Shared/_PublicLayout.cshtml`: yeni küçük hareket betiğinin yüklenmesi.
- `Views/Shared/Public/_ServiceCard.cshtml`, `_BlogCard.cshtml`: aynı rota sonucunu kullanan görsel bağlantıları.
- `Presentation/PublicReviews.cs`, `ViewComponents/MainLayout/TestimonialsViewComponentPartial.cs`: boş izin listesinin kaldırılması, mevcut salt okunur yorum yolu.
- `Views/Shared/Components/SubscribeViewComponentPartial/Default.cshtml`, `CommentViewComponentPartial/Default.cshtml`: yalnız hedef düğme sınıfı.
- `Views/Shared/Components/LocationMapViewComponentPartial/Default.cshtml`, `wwwroot/public-web/images/turkiye.svg`: iki hizmet ili ve verilen iletişim bilgileri.
- `Views/Shared/Public/_HomeContact.cshtml`, `_ContactInfo.cshtml`, `_Footer.cshtml`: adres/telefon yazımının tutarlılığı.

## Doğrulama

- İlgili web projesi `dotnet build --no-restore` ile ayrı geçici çıktı dizinine derlendi: **0 hata, 108 uyarı**. Normal çıktı DLL'si başka bir yerel süreç tarafından kilitli olduğu için kullanıcının çalışan süreci durdurulmadı. Uyarılar bu turda değiştirilen dosyalardan gelmiyor; mevcut framework/nullability/eski partial kullanımı gibi alanlara ait. Framework yükseltilmedi.
- Chrome/Playwright önizlemesinde **160 kontrol kaydı** geçti: 126 genel rota/arayüz, 22 iyileştirme senaryosu, 10 uç durum ve 2 ek odak/bekleyen görsel kontrolü. Konsol hatası ve eksik yerel asset yok.
- **360, 390, 768, 1024, 1440 ve 1920 px** genişliklerde bölüm sırası, taşma, görsel/bağlantı bütünlüğü ve responsive davranış doğrulandı. Yatay taşma yok.
- Animasyonlar çalışan tarayıcıda zamana bağlı opaklık/yükseklik örnekleri ve gerçek kare kaydıyla izlendi. Ana geçiş iki yönlü, hızlı gezinme, iptal/fallback, yüklemesi bekleyen görsel, SSS açılış/kapanış/seri tıklama, yorum 0/1/2/çok/uzun kayıt, yeniden boyutlandırma, azaltılmış hareket, touch ve JavaScript kapalı senaryolar kontrol edildi. Statik ekran görüntüleri animasyon kanıtı olarak kullanılmadı.
- Fixture kullanmayan gerçek uygulamada iletişim, iş başvurusu, gizlilik ve giriş GET sayfaları 200 döndü. İlgili form sayfalarında antiforgery mevcut; giriş sayfasına public CSS sızmıyor. Ana sayfa GET'i erişilebilir veritabanı bulunmadığından beklenen 500 sonucunu verdi; bu ortam sınırlaması devam ediyor.
- Gerçek form/yorum gönderimi, dosya yükleme veya e-posta gönderimi yapılmadı. Yükleme arayüzünün başarı/hata yanıtları yalnız tarayıcıda yakalanan sahte yanıtlarla sınandı. Önizlemede harici CAPTCHA betiği çalıştırılmadı; ekran görüntülerinde bu durum yerel önizleme etiketiyle belirtildi. Gerçek CAPTCHA doğrulaması bu çalışmanın testi değildir.
- Başlangıçtaki **1.844 dosyanın SHA-256 karşılaştırması**: yalnız hedef public sunum dosyaları, dar yorum bileşeni ve raporlar değişti. API, Flutter/mobile, authentication/admin/customer, controller, ortak business/data/entity, migration ve yapılandırma dosyalarına bu turda dokunulmadı. Önceden var olan kullanıcı değişiklikleri korundu; nested mobil Git çalışma ağacı temiz. Diff whitespace kontrolü geçti.

## Görsel kanıtlar

Ekran görüntüleri mevcut gerçek Razor görünümlerini geçici yerel veriyle çalıştıran önizlemeden alınmıştır; yorum ekranlarındaki `TEST` kayıtları gerçek müşteri beyanı değildir.

- [Masaüstü ana sayfa](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/home-full-1440.png)
- [Telefon ana sayfa](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/home-full-390.png)
- [Masaüstü harita ve iletişim](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/map-1440.png)
- [Telefon harita ve iletişim](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/map-390.png)
- [Masaüstü yorum kaydırıcısı — test verisi](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/review-carousel-test-1440.png)
- [Telefon yorum kaydırıcısı — test verisi](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/review-carousel-test-390.png)
- [Ana kaydırıcı, SSS ve yorum geçişlerinin gerçek tarayıcı kaydı — GIF](C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-3/motion-demo.gif)

Aynı dış `revision-3` dizininde derleme günlüğü, başlangıç hash listesi, kapsam kontrolü ve tarayıcı sonuç JSON dosyaları bulunur. Geçici test projesi, fixture verileri ve kayıt araçları uygulama deposuna eklenmedi.
