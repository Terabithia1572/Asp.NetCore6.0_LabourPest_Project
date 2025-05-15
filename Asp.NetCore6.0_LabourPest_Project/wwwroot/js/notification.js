let connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationhub")
    .build();

function loadNotifications() {
    $.get('/Notification/GetLatestNotifications', function (data) {
        $('#notificationList').html(data);
    });
}

connection.on("ReceiveNotification", function () {
    loadNotifications();
});

connection.start().then(function () {
    loadNotifications();
    console.log("✅ SignalR bağlantısı kuruldu.");
}).catch(function (err) {
    console.error("❌ SignalR bağlantı hatası:", err.toString());
});
