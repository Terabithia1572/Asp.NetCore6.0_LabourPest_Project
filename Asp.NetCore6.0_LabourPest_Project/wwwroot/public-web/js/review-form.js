(() => {
    "use strict";
    const form = document.querySelector("[data-review-form]");
    if (!form) return;
    const photo = form.querySelector("#review-photo");
    const path = form.querySelector("#review-image-url");
    const uploadStatus = form.querySelector("#review-upload-status");
    const submit = form.querySelector('[type="submit"]');
    const slot = form.querySelector("[data-review-captcha]");
    const status = form.querySelector("#review-captcha-status");
    const retry = form.querySelector("[data-captcha-retry]");
    let widgetId, widgetSize, generation = 0, verified = false, uploading = false, submitting = false;
    let loading, loadFailed = false, observer;
    const update = () => { submit.disabled = !verified || uploading || submitting; };
    const size = () => {
        const css = getComputedStyle(form);
        return form.clientWidth - parseFloat(css.paddingLeft) - parseFloat(css.paddingRight) >= 304 ? "normal" : "compact";
    };
    function verificationError(message) {
        verified = false;
        status.textContent = message;
        retry.hidden = false;
        update();
    }
    function render() {
        if (!window.grecaptcha?.render || submitting) return;
        const nextSize = size();
        if (widgetId !== undefined && nextSize === widgetSize) return;
        const ticket = ++generation;
        if (widgetId !== undefined) {
            // Only a normal/compact boundary change recreates our own container.
            // Reset the old token first; callbacks from the old widget are ignored.
            grecaptcha.reset(widgetId);
        }
        widgetId = undefined;
        verified = false;
        update();
        slot.replaceChildren();
        slot.style.width = nextSize === "normal" ? "304px" : "164px";
        const host = document.createElement("div");
        slot.append(host);
        try {
            widgetId = grecaptcha.render(host, {
                sitekey: slot.dataset.sitekey, size: nextSize,
                callback: () => { if (ticket !== generation) return; verified = true; status.textContent = "Doğrulama tamamlandı."; retry.hidden = true; update(); },
                "expired-callback": () => { if (ticket === generation) verificationError("Doğrulamanın süresi doldu. Lütfen yeniden doğrulayın."); },
                "error-callback": () => { if (ticket === generation) verificationError("Doğrulama yüklenemedi. Bağlantınızı kontrol edip yeniden deneyin."); }
            });
            widgetSize = nextSize;
            status.textContent = "Robot doğrulaması yükleniyor…";
            const frame = host.querySelector('iframe[title="reCAPTCHA"]');
            frame?.addEventListener("load", () => {
                if (ticket === generation && !verified) status.textContent = "";
            }, { once: true });
            retry.hidden = true;
        } catch { verificationError("Doğrulama başlatılamadı. Lütfen yeniden deneyin."); }
    }
    function loadScript() {
        if (window.grecaptcha?.render) return Promise.resolve();
        if (window.labourPestCaptchaLoading) return window.labourPestCaptchaLoading;
        window.labourPestCaptchaLoading = new Promise((resolve, reject) => {
            const script = document.createElement("script");
            let completed = false;
            const timer = setTimeout(() => done(false), 20000);
            function done(ok) {
                if (completed) return;
                completed = true;
                clearTimeout(timer);
                if (ok) resolve();
                else { script.remove(); window.labourPestCaptchaLoading = null; reject(new Error("captcha-unavailable")); }
            }
            window.labourPestCaptchaReady = () => done(true);
            script.src = "https://www.google.com/recaptcha/api.js?onload=labourPestCaptchaReady&render=explicit&hl=tr";
            script.async = true;
            script.onerror = () => done(false);
            document.head.append(script);
        });
        return window.labourPestCaptchaLoading;
    }
    async function prepare() {
        if (loading || widgetId !== undefined || loadFailed) return;
        observer?.disconnect();
        status.textContent = "Robot doğrulaması yükleniyor…";
        loading = loadScript();
        try { await loading; render(); }
        catch { loadFailed = true; verificationError("Doğrulama yüklenemedi. Bağlantınızı kontrol edip yeniden deneyin."); }
        finally { loading = null; }
    }
    retry.addEventListener("click", () => {
        if (widgetId !== undefined) {
            grecaptcha.reset(widgetId); verified = false; update();
            status.textContent = "Lütfen robot doğrulamasını yeniden tamamlayın."; retry.hidden = true;
        } else { loadFailed = false; prepare(); }
    });
    form.addEventListener("focusin", prepare);
    if ("IntersectionObserver" in window) {
        observer = new IntersectionObserver(entries => { if (entries.some(e => e.isIntersecting)) prepare(); }, { rootMargin: "800px" });
        observer.observe(form);
    } else prepare();
    let resizeTimer;
    if ("ResizeObserver" in window) new ResizeObserver(() => {
        clearTimeout(resizeTimer);
        if (widgetId !== undefined && size() !== widgetSize) resizeTimer = setTimeout(render, 180);
    }).observe(form);

    form.addEventListener("submit", event => {
        if (submitting || uploading || !verified || widgetId === undefined || !grecaptcha.getResponse(widgetId)) {
            event.preventDefault();
            if (!submitting && !uploading) { status.textContent = "Lütfen robot doğrulamasını tamamlayın."; status.focus(); prepare(); }
            return;
        }
        // Normal MVC POST: HTML errors, redirect only after successful persistence.
        submitting = true;
        update();
        submit.setAttribute("aria-busy", "true");
    });
    window.addEventListener("pageshow", event => {
        if (!event.persisted) return;
        submitting = false; verified = false; submit.removeAttribute("aria-busy");
        if (widgetId !== undefined) grecaptcha.reset(widgetId);
        update();
    });
    photo.addEventListener("change", async () => {
        path.value = "";
        const file = photo.files[0];
        if (!file) { uploadStatus.textContent = ""; return; }
        uploading = true; update(); photo.disabled = true;
        uploadStatus.textContent = "Fotoğraf yükleniyor…";
        const abort = new AbortController();
        const timer = setTimeout(() => abort.abort(), 20000);
        try {
            const data = new FormData();
            data.append("file", file);
            data.append("__RequestVerificationToken", form.querySelector('[name="__RequestVerificationToken"]').value);
            const response = await fetch(form.dataset.uploadUrl, { method: "POST", body: data, signal: abort.signal });
            if (!response.ok) throw new Error("upload");
            const result = await response.json();
            if (typeof result.filePath !== "string" || !/^\/canabicom\/profilePhoto\/[A-Za-z0-9_-]+\.(png|jpe?g|webp|gif|avif)$/i.test(result.filePath)) throw new Error("path");
            path.value = result.filePath;
            uploadStatus.textContent = "Fotoğraf yüklendi.";
        } catch {
            photo.value = "";
            uploadStatus.textContent = "Fotoğraf yüklenemedi. Yeniden deneyebilir veya fotoğrafsız gönderebilirsiniz.";
        } finally {
            clearTimeout(timer); uploading = false; photo.disabled = false; update();
        }
    });
    const feedback = document.querySelector("[data-review-feedback]");
    if (feedback) { feedback.focus({ preventScroll: true }); feedback.scrollIntoView({ block: "center", behavior: "instant" }); }
})();
