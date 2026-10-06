# Public web görsel türevleri

Bu araç yalnızca build sırasında çalışır; Node.js uygulama sunucusunda gerekli değildir.

Bu dizinde `npm ci` ardından `npm run build` çalıştırın. Kilit dosyası Sharp sürümünü sabitler. Çıktılar web projesinin `wwwroot/public-web/images/responsive` dizinine yazılır ve normal web yayınına dahil olur. İlk üretimde 10 kaynak için 29 WebP türevi ve bir manifest oluşturuldu.

Araç slider ve hizmet görsel dizinlerini tarar; hakkında görseli ve küçük logoyu da işler. Kaynak dosyaları veya veritabanındaki yolları değiştirmez. Kalite 82, en büyük genişlik 1280 px; küçük kaynaklar büyütülmez. Logo 96 px türevi kullanır.

`PublicImageVariants` kaynak SHA-256, dosya boyutu ve değişiklik zamanını kontrol eder. Yeni bir yönetim paneli görseli, değişmiş bir kaynak veya eksik türev varsa Razor mevcut orijinal `src` yoluna döner. Türevler güncellendiğinde uygulamanın manifesti tekrar okuması için süreç yeniden başlatılmalıdır. Üretim aracı eski hash dosyalarını otomatik silmez; mevcut önbellek bağlantıları korunur.

Bu çalışma hosting önbelleği veya sıkıştırma ayarlarını değiştirmez.
