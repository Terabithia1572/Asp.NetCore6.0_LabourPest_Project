$(document).ready(function () {
    $('.search-trigger').on('click', function (e) {
        e.preventDefault();
        $('.search-panel').toggleClass('active');
    });

    // Dropdown d���na t�kland���nda kapanma
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.widget_search').length) {
            $('.search-panel').removeClass('active');
        }
    });
});
document.addEventListener('DOMContentLoaded', function () {
    const searchTrigger = document.querySelector('.search-trigger');
    const searchPanel = document.querySelector('.search-panel');

    searchTrigger.addEventListener('click', function (e) {
        e.preventDefault();
        searchPanel.classList.toggle('active');
    });

    // Dropdown d���na t�kland���nda kapanma
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.widget_search')) {
            searchPanel.classList.remove('active');
        }
    });
});
