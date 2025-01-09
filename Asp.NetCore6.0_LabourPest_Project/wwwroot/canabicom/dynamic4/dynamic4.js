<script>
    $(document).ready(function () {
        $('#partners-carousel').owlCarousel({
            loop: true, // Sonsuz d�ng�
            margin: 20, // Her resim aras�ndaki bo�luk
            autoplay: true, // Otomatik oynatma
            autoplayTimeout: 3000, // 3 saniyede bir kayd�rma
            autoplayHoverPause: true, // Fareyle �zerine gelince durdurma
            responsive: {
                0: {
                    items: 2 // K���k ekranlarda 2 resim
                },
                600: {
                    items: 3 // Orta ekranlarda 3 resim
                },
                1000: {
                    items: 6 // B�y�k ekranlarda 6 resim
                }
            }
        });
    });
</script>