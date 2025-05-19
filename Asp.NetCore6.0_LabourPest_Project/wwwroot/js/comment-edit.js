document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".comment-edit-btn").forEach(function (btn) {
        btn.addEventListener("click", function () {
            const commentId = this.getAttribute("data-edit-id");
            const form = document.getElementById("editForm-" + commentId);
            if (form) {
                form.style.display = "block";
            }
        });
    });
});

window.cancelEdit = function (commentId) {
    const form = document.getElementById("editForm-" + commentId);
    if (form) {
        form.style.display = "none";
    }
};

window.submitEdit = function (commentId) {
    const title = document.getElementById("editTitle-" + commentId).value;
    const content = document.getElementById("editContent-" + commentId).value;

    const formData = new URLSearchParams();
    formData.append("id", commentId);
    formData.append("title", title);
    formData.append("content", content);

    fetch('/Blog/UpdateCommentAjax', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const container = document.getElementById("comment-" + commentId);
                container.querySelector(".comment-title").textContent = data.updatedTitle;
                container.querySelector(".comment-text").textContent = data.updatedContent;
                container.querySelector("time").textContent = data.updatedDate;
                cancelEdit(commentId);
            } else {
                alert("Güncelleme başarısız: " + data.message);
            }
        })
        .catch((error) => {
            console.error("Fetch error:", error);
            alert("Sunucu hatası oluştu.");
        });
};
