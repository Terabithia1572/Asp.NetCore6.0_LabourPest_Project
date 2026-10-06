# Public web yayın paketi — 6 Ekim 2026

Bu paket tamamlanan public web tasarımını, responsive düzeni, animasyonları, yorum gönderimi düzeltmesini, CAPTCHA entegrasyonunu ve görsel/performance iyileştirmelerini GitHub için bir araya getirir. Ayrı üretim deployment'ı yapılmadı.

## Kapsam

Mevcut API, Flutter, auth/admin/customer ekranları, ortak business/data/entity değişiklikleri ve özel config dosyaları commit dışında bırakıldı. Yereldeki önceki çalışmalar korunuyor. `Program.cs` yalnız public görsel yardımcısı ve mevcut yorum servisi/CAPTCHA DI kayıtlarıyla kısmi olarak stage edildi; ilgisiz yerel DI ve middleware düzenlemeleri alınmadı. Nested mobil Git deposu/submodule eklenmedi.

Uzak Program sürümündeki güvenlik başlıkları Google kaynaklarını engellediğinden izinler yalnız `_PublicLayout` yanıtına uygulandı. Public CSP gerekli Google reCAPTCHA adreslerine izin verir; form-action ve base-uri self ile sınırlıdır. Public yanıtın COEP başlığı kaldırılır, COOP same-origin-allow-popups kullanır. Authenticated portal middleware politikası bu commit ile değiştirilmez.

Testler yayınlanmayacak yerel Context constructor/configuration değişikliklerine bağımlı değildir. Test assembly'sindeki EF Core ConnectionOpening gözlemcisi gerçek repository bağlantısını açılmadan önce rastgele isimli LocalDB test veritabanına yönlendirir. Uygulama assembly'sine test yönlendirmesi veya CAPTCHA dublörü eklenmez.

## Doğrulama

Stage edilen ağaç ayrı bir dizine çıkarılarak doğrulandı; ilgili olmayan yerel kaynaklar bu doğrulama kopyasına alınmadı.

- Public web Release build: 0 hata; .NET 6 destek süresi uyarısı mevcut. İlk derlemede mevcut nullable/legacy uyarıları da görüldü.
- İzole SQL LocalDB regresyonları: **36 geçti, 0 başarısız, 0 atlanan**. Önceki 35 teste public layout güvenlik başlığı kontrolü eklendi.
- Önceki **160 tarayıcı kontrolü** ilgili tasarım/etkileşimler için korundu; yeni performans ölçümü çalıştırılmadı.
- Yayınlanacak Razor ve eski host başlıklarını kullanan yerel GET önizlemesinde gerçek Google CAPTCHA 390 ve 1440 px'de yeniden açıldı: tek script, normal 304×78 widget, taşma/console hatası yok. Üretime POST veya challenge gönderilmedi.
- README'deki 16 yerel bağlantı referansı mevcut; dört benzersiz ekran görüntüsünün toplamı yaklaşık 2,99 MB. Staged whitespace kontrolü geçti.
- Staged dosyalar API, Flutter, ortak backend veya özel config değişikliği içermiyor.
- Yayınlanacak yeni dosya içeriklerinde credential/token/private-key kontrolünde bulgu yok. Gerçek reCAPTCHA secret yeni içerikte bulunmuyor.

5 Ekim tarihli [teknik rapor](public-web-review-performance.md), önceki yerel 35 test ve Lighthouse ölçümlerinin tarihsel kaydıdır; bu sonlandırma paketindeki 36 test sonucu onun yerine yeni performans ölçümü anlamına gelmez.

## README ve ekranlar

README önce Türkçe, ardından İngilizcedir. MVC, API ve ayrı Flutter projesi açıkça ayrılmıştır. Doğrulanmış stack, özellikler, kurulum/build/test komutları, mevcut SQL Server bağlantı davranışı ve placeholder CAPTCHA ayarları açıklanır. Yunus İNAN atfı korunur.

`screenshots/` altında dört güncel public ekran vardır: ana sayfa, hizmetler, galeri ve iletişim/hizmet alanları. Orijinal dosyalar taşınmadı veya silinmedi. Yönetim/müşteri ekranları ile isim/sağlık bilgisi içeren yorum görüntüleri yayınlanmadı. Mobil veya PageSpeed ekranı sağlanmadığından uydurulmadı.

Mobil/masaüstü PageSpeed 100/100, **kullanıcı bildirimi** olarak yazıldı; tüm kategoriler için 100 veya gelecekte sabit skor iddiası yok.

## GitHub ve yapılandırma

Hedef `Terabithia1572/Asp.NetCore6.0_LabourPest_Project`, varsayılan dal `master`. Push öncesi fetch sonrasında local/remote divergence 0/0 idi. GitHub API kontrolünde Actions workflow, webhook ve deployment kayıtları boş, master protected=false idi. Görülen bir otomatik deployment tetikleyicisi yok; harici sunucu polling'i bu kontrolün kapsamı dışındadır.

Üretimde mevcut site key ile eşleşen `Recaptcha__SecretKey` güvenli sunucu yapılandırmasında bulunmalıdır. Bu görev üretim ayarlarını veya verilerini değiştirmez. Gerçek Google challenge/server verification testi yapılmadı.

**Geçmiş secret notu:** GitHub'daki önceki `MainCommentController` sürümünde kod içinde reCAPTCHA secret bulundu. Yeni controller bunu configuration'dan okur. Değer tekrar yazılmadı; Git geçmişi değiştirilmedi. Daha önce yayımlanan anahtarın sahibi tarafından yenilenmesi ve yeni değerin yalnız güvenli sunucu ayarına konması gerekir.
