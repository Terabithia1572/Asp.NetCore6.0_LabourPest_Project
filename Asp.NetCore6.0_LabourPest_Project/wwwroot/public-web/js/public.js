(() => {
    "use strict";
    const root = document.querySelector(".lp-public");
    if (!root) return;
    const toggle = root.querySelector(".lp-menu-toggle");
    const nav = root.querySelector("#public-navigation");
    const mobile = window.matchMedia("(max-width: 980px)");
    let previousOverflow = "";
    let menuOpen = false;
    function setMenu(open, returnFocus = false) {
        const wasOpen = menuOpen;
        menuOpen = open && mobile.matches;
        nav.classList.toggle("is-open", menuOpen);
        toggle.setAttribute("aria-expanded", String(menuOpen));
        toggle.querySelector("[data-menu-label]").textContent = menuOpen ? "Kapat" : "Menü";
        toggle.querySelector(".lp-menu-symbol").textContent = menuOpen ? "×" : "☰";
        if (menuOpen && !wasOpen) { previousOverflow = document.body.style.overflow; document.body.style.overflow = "hidden"; }
        if (!menuOpen && wasOpen) document.body.style.overflow = previousOverflow;
        if (returnFocus && mobile.matches) toggle.focus();
    }
    if (toggle && nav) {
        root.classList.add("lp-menu-ready");
        toggle.hidden = false;
        toggle.addEventListener("click", () => setMenu(!menuOpen, menuOpen));
        nav.addEventListener("click", event => { if (event.target.closest("a")) setMenu(false, true); });
        document.addEventListener("keydown", event => {
            if (event.key === "Escape" && menuOpen) { event.preventDefault(); setMenu(false, true); }
        });
        // This is an in-flow disclosure, not a modal: tabbing beyond it closes it.
        document.addEventListener("focusin", event => { if (menuOpen && !event.target.closest(".lp-header")) setMenu(false); });
        document.addEventListener("pointerdown", event => { if (menuOpen && !event.target.closest(".lp-header")) setMenu(false); });
        mobile.addEventListener("change", () => setMenu(false));
    }
    root.querySelectorAll(".lp-disclosure").forEach(details => {
        const summary = details.querySelector("summary");
        const sync = () => summary.setAttribute("aria-expanded", String(details.open));
        sync();
        details.addEventListener("toggle", sync);
    });
    const fallback = root.querySelector(".lp-lightbox img")?.getAttribute("src");
    root.querySelectorAll("img[data-fallback]").forEach(img => {
        const replace = () => {
            if (!fallback || img.getAttribute("src") === fallback) return;
            img.src = fallback;
            img.removeAttribute("srcset");
        };
        img.addEventListener("error", replace, { once: true });
        if (img.hasAttribute("src") && img.complete && img.naturalWidth === 0) replace();
    });
    const dialog = root.querySelector(".lp-lightbox");
    if (dialog && typeof dialog.showModal === "function") {
        let opener, oldOverflow;
        root.querySelectorAll("a[data-gallery]").forEach(link => link.addEventListener("click", event => {
            if (event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
            event.preventDefault();
            opener = link;
            const thumbnail = link.querySelector("img");
            dialog.querySelector("img").src = thumbnail.currentSrc || link.href;
            dialog.querySelector("img").alt = thumbnail.alt;
            dialog.querySelector("#gallery-caption").textContent = thumbnail.alt;
            oldOverflow = document.body.style.overflow;
            document.body.style.overflow = "hidden";
            dialog.showModal();
        }));
        dialog.addEventListener("click", event => {
            if (event.target !== dialog) return;
            const rect = dialog.getBoundingClientRect();
            if (event.clientX < rect.left || event.clientX > rect.right || event.clientY < rect.top || event.clientY > rect.bottom) dialog.close();
        });
        dialog.addEventListener("close", () => {
            document.body.style.overflow = oldOverflow || "";
            opener?.focus();
        });
    }
    window.setReplyTo = commentId => {
        const parent = document.getElementById("ParentCommentID");
        const input = document.getElementById("comment234");
        if (parent && input) { parent.value = commentId; input.focus(); }
        return false;
    };
})();
