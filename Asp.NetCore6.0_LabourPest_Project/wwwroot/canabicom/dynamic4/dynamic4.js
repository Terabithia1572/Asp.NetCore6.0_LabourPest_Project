<script>
    $(document).ready(function () {
        $('#partners-carousel').owlCarousel({
            loop: true, // Sonsuz döngü
            margin: 20, // Her resim arasýndaki boþluk
            autoplay: true, // Otomatik oynatma
            autoplayTimeout: 3000, // 3 saniyede bir kaydýrma
            autoplayHoverPause: true, // Fareyle üzerine gelince durdurma
            responsive: {
                0: {
                    items: 2 // Küçük ekranlarda 2 resim
                },
                600: {
                    items: 3 // Orta ekranlarda 3 resim
                },
                1000: {
                    items: 6 // Büyük ekranlarda 6 resim
                }
            }
        });
    });
</script>