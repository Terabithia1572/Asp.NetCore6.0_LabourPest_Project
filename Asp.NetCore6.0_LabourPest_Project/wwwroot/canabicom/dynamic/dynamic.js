document.addEventListener("DOMContentLoaded", () => {
    gsap.registerPlugin(ScrollTrigger);

    // Tüm section'ları seçiyoruz
    const sections = document.querySelectorAll("section");

    // Her bir section için animasyon oluştur
    sections.forEach((section, index) => {
        gsap.fromTo(
            section,
            { opacity: 0, y: 50 }, // Başlangıç animasyonu (görünmez ve aşağıda)
            {
                opacity: 1,
                y: 0, // Hedef (görünür ve yukarıya gelmiş)
                duration: 1.2, // Animasyon süresi
                ease: "power4.out",
                scrollTrigger: {
                    trigger: section, // Bu section görünce animasyon başlar
                    start: "top 80%", // Animasyonun başlaması
                    toggleActions: "play none none reverse", // Scroll hareketleri
                },
            }
        );
    });
});
ScrollTrigger.defaults({
    markers: false, // ScrollTrigger işaretlerini gösterme
    scrub: 0.5, // Animasyonu scroll hareketine bağlar
    end: "bottom top", // Bitirme konumu
});
