# LabourPest yorum gönderimi, CAPTCHA ve performans

5 Ekim 2026. Değişiklikler yerelde tamamlandı; push, merge veya deployment yapılmadı. Onaylanan public tasarım, 15 bölümün sırası, içerikler ve animasyonlar korundu.

## Doğrulanan hata ve düzeltme

Ana sayfa formu normal HTML POST ile mevcut `MainCommentController.AddComment` action'ına gidiyor. `CommentController` yorum yönetimi için aynı Comment kaydını kullanıyor; `BlogCommentController` ayrı blog yorum akışıdır. Yeni controller veya kayıt servisi oluşturulmadı.

Eski formdaki isteğe bağlı `ImageURL` boş olduğunda MVC binder, `Comment.ImageUrl` değerini null yapıyor. Büyük/küçük harf farkı binding hatası değil. Paylaşılan EF modelinde ve mevcut şema tanımında ImageUrl zorunlu, SQL sütunu `nvarchar(max) NOT NULL`. Eski action ModelState'i kontrol etmeden kayıt yapıyordu.

Gerçek MVC model binding ve gerçek `CommentManager → EfCommentRepository → GenericRepository<Comment>.Insert → Context.SaveChanges` akışı, rastgele isimli izole SQL LocalDB veritabanında tekrarlandı:

- Fotoğraf yolu boş: `DbUpdateException`, iç hata `SqlException`, SQL hata numarası **515**, sütun **ImageUrl**, eklenen kayıt **0**.
- Geçerli yerel fotoğraf yolu: eklenen kayıt **1**.

Tekrarlanan hata **INSERT sırasında, başarılı persistence tamamlanmadan önce** oluşuyor. Üretimdeki aynı isteğe ait exception kaydı yok; dolayısıyla bunun canlıdaki tek olası hata olduğunu söylemiyoruz. Mevcut yerel uygulama logları bu isteğe ait kullanılabilir bir stack trace sağlamadı. Yerel kanıt düzeltme için yeterli; şu aşamada kullanıcıdan üretim logu istenmiyor.

Web'e özel `PublicReviewInput`, persistence entity'sine doğrudan binding'i kaldırıyor. Fotoğraf isteğe bağlı kaldı. Eksik fotoğraf yalnız web mapping'inde mevcut `/labourpestcustomer/assets/img/team/avatar-placeholder.webp` dosyasına çevriliyor; paylaşılan entity, şema ve API değişmedi. Fotoğraf verilirse mevcut `/canabicom/profilePhoto/...` yükleme sözleşmesi korunuyor. `UploadImage` action'ının uygulaması değiştirilmedi.

Ad, konu ve mesaj zorunlu; web sınırları sırasıyla 100/200/5000 karakter. Bunlar `nvarchar(max)` veritabanı sütunları üzerinde yeni web giriş sınırlarıdır, mevcut DB uzunlukları gibi sunulmamıştır. ID binding'den çıkarıldı; tarih ve mevcut `CommentStatus = true` davranışı sunucuda atanıyor. Gönderilen status/date/id alanları kaydı belirlemiyor.

Başarı yalnız `TAdd`/`SaveChanges` döndükten sonra kısa TempData mesajı ve `/#comment` yönlendirmesiyle gösteriliyor. Yenileme GET yapıyor. Alan/CAPTCHA hataları 422, güvenlik token'ı reddi 400, doğrulama hizmeti/kayıt hataları 503 ve onaylanan public görünümle dönüyor. Ad, konu ve mesaj HTML-encode edilerek korunuyor; büyük mesajlar TempData cookie'sine yazılmıyor. Dosya seçiminin geri yüklenemeyeceği açıklanıyor. Beklenmeyen kayıt hatasında exception türü, SQL numarası, request trace ve stack konumu içeride loglanıyor; kullanıcıya kaydın doğrulanamadığı söyleniyor. Hata sonrası başarı gösterilmiyor.

İstemcide hızlı çift gönderim kilidi ve başarı sonrası Post/Redirect/Get var. Bunlar sunucuda genel idempotency garantisi değildir; bağlantı kesilmesinde belirsiz commit durumunu kullanıcı mesajı da kabul eder.

## CAPTCHA ve gereken sunucu ayarı

Mevcut Google reCAPTCHA v2 checkbox korundu. `RecaptchaVerifier` secret ve token'ı yalnız POST gövdesinde Google'ın `siteverify` adresine gönderiyor; JSON boolean yapısal olarak okunuyor. Eksik, reddedilen, süresi dolan/yeniden kullanılan token, bozuk JSON, HTTP/ağ hatası ve eksik ayar kayıt yapılmadan reddediliyor. HttpClient timeout'u 8 saniye; aynı token ile otomatik retry yok. Desteklenen akış: [Google sunucu doğrulama belgesi](https://developers.google.com/recaptcha/docs/verify).

**Canlıya alınmadan önce `Recaptcha__SecretKey`, mevcut public site key ile eşleşen değerle sunucunun güvenli yapılandırmasında tanımlanmalı.** `Recaptcha:SecretKey` configuration bölümü de desteklenir. Kod içindeki eski secret kaldırıldı; secret rapora, Razor'a, JavaScript'e veya yeni bir config dosyasına yazılmadı. Üretim anahtarları döndürülmedi/değiştirilmedi. Public site key aynı kaldı; ayrı geliştirme ortamında gerektiğinde `Recaptcha__SiteKey` ve eşleşen secret birlikte sağlanmalı. Üretime test anahtarı konulmadı. Ayar yoksa doğrulama kapalı kalır ve yorum kaydedilmez.

SDK form görünümüne 800 px yaklaşınca veya klavye odağı forma gelince tek kez yükleniyor; doğrudan `#comment` bağlantısı da çalışıyor. Yüklenme, ağ hatası, süre dolması ve tekrar deneme durumları Türkçe. Doğrulama tamamlanana kadar gönderim kapalı. Sunucu kontrolü her durumda zorunlu.

Gerçek kullanılabilir form genişliği en az 304 px ise normal widget, daha darsa compact kullanılıyor. Widget ID 0 dahil saklanıyor; reset/getResponse doğru ID ile çağrılıyor. Yalnız normal/compact sınırı geçildiğinde yeniden oluşturuluyor, önce eski token sıfırlanıyor ve eski callback'ler yok sayılıyor. İframe içeriği değiştirilmedi; marka gizlenmedi, kırpma veya transform ölçekleme yapılmadı. [Google desteklenen boyut ve JavaScript API belgesi](https://developers.google.com/recaptcha/docs/display).

Gerçek Google widget'ı 360, 390, 768, 1024 ve 1440 px genişliklerde açıldı. 360 px ekranda formun içi 278 px olduğundan compact 164×144; 390 px ve üzerindeki kontrollerde normal 304×78 kullanıldı. 1440 px'de düğme aynı satırda; diğer dar form genişliklerinde ayrı satırda. Hepsinde yatay taşma yok, bir SDK script'i var, doğrulanmamış gönderim kapalı.

- [Telefon, 390 px — normal widget](public-web-review-evidence/captcha-390.png)
- [Dar telefon, 360 px — compact widget](public-web-review-evidence/captcha-360.png)
- [Masaüstü, 1440 px](public-web-review-evidence/captcha-1440.png)
- [Beş genişliğin gerçek widget ölçümleri](public-web-review-evidence/captcha-layout-results.json)

Bu görüntülerde yalnız tarayıcıdaki labourpest.com hostname'i 127.0.0.1'e yönlendirilerek yerel önizleme açıldı; cevap adresi doğrulandı. Google widget'ı gerçektir, siteye POST gönderilmedi. Checkbox/challenge tamamlanmadı. Bu kontrol **gerçek Google sunucu doğrulamasının veya üretim key pair'inin uçtan uca test edildiği anlamına gelmez**.

## Test sonuçları

`dotnet test tests/PublicWeb.Tests/PublicWeb.Tests.csproj -c Release`: **35 geçti, 0 başarısız, 0 atlanan**. İzole LocalDB ile gerçek persistence kullanıldı; Google sonucu yalnız test host'unda kontrollü verifier ile sağlandı.

- Eski hata için 2 binding/persistence testi.
- Fotoğraflı/fotoğrafsız kayıt, tek kayıt, server-owned alanlar, PRG/yenileme, geçersiz/uzun giriş, CAPTCHA reddi/timeout/ayar eksikliği, antiforgery ve gerçek SQL trigger ile kayıt hatası dahil 17 MVC testi.
- HTTP doğrulayıcının JSON, HTTP, timeout, eksik ayar ve retry yapmama davranışları için 15 test.
- Yönetim panelinden değişen/yeni görsel ve eksik türev için 1 fallback testi.

Fotoğraflı test mevcut yükleme cevabındaki yerel yol sözleşmesini kullanır. Tarayıcı testlerinde yükleme cevabı kontrollüdür; gerçek üretim dosya yüklemesi yapılmadı. Verifier testleri Google insan doğrulamasının yerine geçmez. Geçici test veritabanlarının son sayısı **0**. [Testleri tekrar çalıştırma](../tests/PublicWeb.Tests/README.md).

Tarayıcı kontrolleri: **126 sayfa/yerleşim + 22 etkileşim + 12 yorum/CAPTCHA = 160 geçti**; console error, eksik kaynak veya başarısız kontrol yok. 360, 390, 768, 1024, 1440 ve genel sayfalarda ayrıca 1920 px denendi. Slider geçişleri/kararlı yükseklik, hızlı yön değiştirme, hover/focus, FAQ animasyonu, galeri, harita/linkler, reduced motion, JavaScript kapalı görünüm ve form hata durumları korundu. Bu kontrollü testlerdeki CAPTCHA dublörü yalnız test aracında bulunur; gerçek widget kontrolü yukarıda ayrı raporlanmıştır.

[Sayfa kontrolleri](public-web-review-evidence/browser-results.json), [etkileşimler](public-web-review-evidence/enhancement-results.json), [yorum tarayıcı kontrolleri](public-web-review-evidence/review-browser-results.json).

Public web Release build: **0 hata, 108 uyarı**. Mevcut .NET 6/paket, nullable ve eski view uyarıları duruyor; yeni yorum/CAPTCHA/görsel yardımcı dosyalarında yeni compiler uyarısı yok. Değiştirilen üç JavaScript dosyasının syntax kontrolü ve bu revizyonun dosyalarında `git diff --check` geçti. Depo genelindeki önceki admin/config whitespace hatalarına dokunulmadı.

## Performans yöntemi ve sonuçları

Lighthouse **13.5.0**, Headless Chrome **154.0.0.0**, aynı bilgisayar; her profil için üç bağımsız tarayıcı koşusu. Başlangıç 4 Ekim, son ölçüm 5 Ekim 2026. Mobil 412×823, DPR 1,75; varsayılan simulated profil: RTT 150 ms, throughput 1638,4 Kbps, CPU ×4. Masaüstü RTT 40 ms, throughput 10240 Kbps, CPU ×1. Tam ayarlar ve tüm koşular [ölçüm JSON'unda](public-web-review-evidence/performance-summary.json).

FCP/LCP/Speed Index/TBT, Lighthouse simulated laboratuvar metrikleridir. Aşağıdaki gözlenen navigation TTFB, trace'teki LCP breakdown'dan; sunucu yanıtı ise ayrı `server-response-time` audit'inden gelir. Bu iki süre birbirinin yerine kullanılmadı. Field/CrUX kullanıcı verisi yoktur.

**Canlı başlangıç — https://labourpest.com/, yalnız okuma:**

- Mobil skorlar 94 / 93 / 94, medyan **94**. FCP 1,076 sn; LCP 2,505 sn; Speed Index 4,979 sn; TBT 64 ms; CLS 0. Gözlenen navigation TTFB 3.099 ms; audit sunucu yanıtı 30 ms. Toplam 1.042.479 B; görsel 516.837 B; JS 369.178 B; CSS 47.820 B; üçüncü taraf 487.101 B.
- Masaüstü skorlar 96 / 96 / 96, medyan **96**. FCP 0,324 sn; LCP 0,614 sn; Speed Index 2,060 sn; TBT 0; CLS 0. Gözlenen navigation TTFB 3.096 ms; sunucu yanıtı 27 ms. Toplam 1.720.987 B; görsel 1.195.362 B; JS 369.172 B; CSS 47.801 B; üçüncü taraf 487.058 B.
- Erişilebilirlik / Best Practices / SEO: her iki profil **100 / 100 / 100**. Kullanıcının önceki 70/56 ve Best Practices 96 değerleri bu ortamda tekrarlanmadı. Eski raporun başarısız audit'i olmadan neden uydurulmadı.

Son ağ kontrolünde navigation TTFB 3.156 ms'nin yaklaşık **3.100 ms'si DNS**, TCP+TLS yaklaşık 19 ms, request→response yaklaşık 36 ms idi. Bu bilgisayar/ağ yolundaki DNS gecikmesi ölçüldü; küresel DNS veya sunucu/veritabanı sorunu olduğu kanıtlanmadı. [Ağ zamanlaması](public-web-review-evidence/production-network-timing.json). Canlı backend'e veya hosting ayarlarına bu bulgu üzerinden müdahale edilmedi.

**Karşılaştırılabilir yerel Release fixture önizlemesi — önce → sonra:**

Önizleme gerçek derlenmiş Razor/public kaynakları kullanır; içerik view component'lerine aynı sabit veri seti verilir, üretim DB'si kullanılmaz. Yerel SQL persistence testleri bundan ayrıdır. Canlı kaynak sıkıştırması, ağ ve verileriyle eşdeğer değildir. Aşağıdaki gelişme yalnız fixture öncesi/sonrası karşılaştırmasıdır; canlıda deployment sonrası skor iddiası değildir.

- Mobil koşular **82 / 92 / 88 → 97 / 97 / 97**, medyan **88 → 97**. FCP **1,053 → 1,202 sn**; LCP **3,527 → 2,477 sn**; Speed Index **1,053 → 1,202 sn**; TBT **71 → 0 ms**; CLS **0,099 → 0**. Gözlenen TTFB **4,405 → 3,830 ms**; sunucu yanıtı **3 → 3 ms**. Son TBT koşuları **0 / 95 / 0 ms**; yalnız medyanın sıfır olması tüm koşuların sıfır olduğu anlamına gelmez.
- Mobil toplam transfer **1.050.358 → 436.028 B**; görseller **517.688 → 324.866 B**; JS **382.408 → 29.609 B**; CSS **69.427 → 33.752 B**; ilk görünümde üçüncü taraf **436.308 → 0 B**.
- Masaüstü koşular **100 / 100 / 99 → 100 / 100 / 100**, medyan **100 → 100**. FCP **0,286 → 0,326 sn**; LCP **0,803 → 0,622 sn**; Speed Index **0,311 → 0,326 sn**; TBT **0 → 0 ms**; CLS **0 → 0**. Gözlenen TTFB **2,871 → 3,764 ms**; sunucu yanıtı **2 → 3 ms**.
- Masaüstü toplam transfer **1.729.432 → 1.045.113 B**; görseller **1.196.760 → 933.951 B**; JS **382.408 → 29.609 B**; CSS **69.427 → 33.752 B**; ilk görünümde üçüncü taraf **436.310 → 0 B**.
- Son altı koşunun tamamında erişilebilirlik / Best Practices / SEO **100 / 100 / 100**. Yerel başlangıç erişilebilirliği 97 idi; görünür link metniyle aria-label uyumu ve boş yorum metni kontrastı düzeltildi.

Mobil LCP yaklaşık **%29,8**, ilk yükleme transferi **%58,5**, görsel transferi **%37,2** azaldı. FCP ve Speed Index az miktarda kötüleşti; bütün metriklerin iyileştiği iddia edilmiyor. Her metrik için üç koşunun ayrı medyanı hesaplandı, en iyi koşu seçilmedi. İlk baseline koşusunda daha yüksek soğuk sunucu yanıtı/CLS vardı; bu koşu sonuçlardan çıkarılmadı.

## Ölçüme dayalı uygulamalar ve kalan sınırlar

LCP elementi her iki ortamda ilk slider görseli: `section#home > div#home-slides > div.lp-banner-slide > img.lp-banner-image`. İlk görsel başlangıç HTML'inde discoverable, eager ve high priority. Sonraki slaytlar ilk belge yüklemesinden sonra hazırlanıyor; geçişten önce decode bekleniyor. Animasyon kaldırılmadı. Hero'nun doğal yüksekliği CSS grid ile ilk boyamadan itibaren ayrılarak mobil CLS giderildi; menü açılışındaki yer değişimi de düzeltildi.

Waterfall'da yaklaşık 358–360 KB reCAPTCHA JavaScript'i ve toplam yaklaşık 487 KB üçüncü taraf maliyeti görüldü. Canlı örnek audit'i bu script'te 184 KiB kullanılmayan kod bildirdi. Kod silinmedi; form yaklaşınca yükleme uygulandı. İlk görünümdeki 0 B üçüncü taraf, **maliyetin ertelenmesidir**, formu kullanan ziyaretçi için yok edilmesi değildir. Lighthouse/user-agent tespiti, güvenlik bypass'ı veya audit'e özel içerik yok. Google'ın font/CSS yükü de ertelendiği için toplam JS/CSS sayıları düştü; first-party CSS'nin yarıya indirildiği iddia edilmiyor.

İlk ölçümde hakkında görseli 118.076 B ve 1941×1440, ilk slider görseli 123.030 B idi; image-delivery yaklaşık 153 KiB tasarruf potansiyeli bildirdi. 10 kaynaktan 29 kalite-82 WebP türevi, responsive `srcset/sizes` ve küçük logo türevi üretildi. Kaynakların toplamı 632.692 B, her kaynağın en büyük türevleri toplamı 420.900 B. Kaynaklar ve DB yolları korunuyor; yeni/değişmiş yönetim paneli görselleri hash kontrolü sayesinde orijinale döner. [Türevleri yeniden üretme](../tools/public-images/README.md).

Ana thread başlangıç mobil örneğinde style/layout ve Google script yürütmesi öne çıktı. Karşılaştırılabilir ikinci yerel koşuda script evaluation yaklaşık 115 → 21 ms, parse/compile 42 → 3 ms oldu; toplam main-thread iş 602 → 585 ms ile sınırlı azaldı. Son koşulardan birindeki 95 ms TBT, mevcut layout/animasyon işinin hâlâ maliyeti olduğunu gösteriyor. Yerel finalde image-delivery ve unused-JS audit'leri geçti; render-blocking public CSS yaklaşık 290 ms tahmini fırsat göstermeye devam ediyor.

Public layout zaten gerekli küçük script'leri kullanıyordu; korunmuş dashboard bağımlılıkları veya talep edilen animasyonlar kaldırılmadı. Onaylanan tipografi sistem fontlarıdır; gözlenen uzaktaki Roboto istekleri reCAPTCHA kaynaklıdır. İlk yükleme için yeni font/preload eklenmedi.

Yerel sunucu statik dosyaları canlıdaki gibi sıkıştırmıyor; örneğin başlangıç public CSS transferi yerelde yaklaşık 33 KB, canlıda yaklaşık 12 KB idi. Hosting için sonraki kontrollü adım: mevcut gzip/Brotli kapsamını doğrulamak, hash/fingerprint'li statik türevlerde uzun cache süresini değerlendirmek ve DNS gecikmesini başka ağdan karşılaştırmak. Bunlar bu çalışmada uygulanmış değişiklikler değildir. Antiforgery/CAPTCHA/feedback taşıyan HTML için blanket full-page cache eklenmedi. Üretim DB gecikmesi fixture'dan ölçülemez. Mobil 100 garanti edilmez; mevcut tutarlı 97/100 sonuçları laboratuvar sonucudur.

## İnceleme kapsamı ve kanıtlar

Önemli dosyalar: `Controllers/MainCommentController.cs`, `Models/PublicReviewInput.cs`, `Presentation/Reviews/*`, `Views/MainComment/AddComment.cshtml`, yorum form component'i, `Program.cs` içindeki web DI kayıtları, `public-web/js/review-form.js`, `public.css`, `carousels.js`, `public.js`, `Presentation/PublicImageVariants.cs`, slider/about/header/service/blog Razor parçaları, `tools/public-images` ve `tests/PublicWeb.Tests`.

Revizyon başlangıcındaki **1.847 dosya SHA-256** ile karşılaştırıldı: değişen mevcut dosyalar yalnız 15 izinli public-web/yorum dosyası. API, ortak business/data/entity, migrations, auth/admin/customer, appsettings ve üretim bağlantı ayarları bu revizyonda değişmedi. Orijinal görseller değişmedi. Flutter ayrı nested Git deposudur; kendi çalışma ağacı temiz, mobil dosyaya yazılmadı. Önceki kullanıcı değişiklikleri geri alınmadı. [Kapsam kontrolü](public-web-review-evidence/scope-audit.json).

Ham Lighthouse JSON/HTML raporları, test TRX ve build logları bu bilgisayarda `C:/Users/Yunus/.codex/visualizations/2026/10/03/01a10170-e1cb-7c71-b9cc-106127c07029/revision-4` altında korunuyor; `production-baseline`, `fixture-before`, `fixture-final` dizinlerinde her profil için üç rapor var. Repoda küçük sonuç JSON'ları ve gerçek CAPTCHA ekran görüntüleri saklandı.

Canlıda sorun sürerse gereken kayıt: sorunlu `POST /MainComment/AddComment` isteğinin saatine yakın, `Asp.NetCore6._0_LabourPest_Project.Controllers.MainCommentController` kategorisindeki **“Public review persistence failed”** satırı; Type, SqlNumber, Trace ve Location alanları. CAPTCHA tarafında aynı zaman aralığındaki `RecaptchaVerifier` mesajının tür/status bilgisi yeterlidir. Sunucunun mevcut logging sağlayıcısından bu satır alınmalı; public detailed error sayfası açılmamalı, secret/token/connection string veya yorum gövdesi paylaşılmamalıdır.
