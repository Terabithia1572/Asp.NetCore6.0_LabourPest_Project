(() => {
    "use strict";
    const motion = window.LabourPestMotion;
    const reduced = motion?.reduced || matchMedia("(prefers-reduced-motion: reduce)");
    const phone = matchMedia("(max-width: 700px)");
    const tablet = matchMedia("(max-width: 980px)");
    const fallback = document.querySelector(".lp-public .lp-lightbox img")?.getAttribute("src");
    const animate = motion?.animate || (() => ({ finished: Promise.resolve(), cancel() {} }));
    document.querySelectorAll(".lp-public [data-carousel]").forEach(carousel => {
        const items = Array.from(carousel.querySelectorAll("[data-carousel-item]"));
        const track = carousel.querySelector("[data-carousel-track]");
        if (!items.length || !track) return;
        const hero = carousel.dataset.carousel === "hero";
        const controls = carousel.querySelector("[data-carousel-controls]");
        const dots = Array.from(carousel.querySelectorAll("[data-carousel-dot]"));
        const counter = carousel.querySelector("[data-carousel-counter]");
        const capacity = () => hero ? 1 : phone.matches ? 1 : tablet.matches ? 2 : 3;
        let perPage = capacity(), active = 0, desired = 0, generation = 0, busy = false, loading = false, animations = [];
        const pages = () => Math.ceil(items.length / perPage);
        const members = index => items.slice(index * perPage, (index + 1) * perPage);
        const wrap = index => ((index % pages()) + pages()) % pages();
        carousel.classList.add("lp-carousel-ready");

        function state(item, visible, interactive = visible) {
            item.hidden = !visible;
            item.inert = !interactive;
            if (interactive) item.removeAttribute("aria-hidden");
            else item.setAttribute("aria-hidden", "true");
            item.style.pointerEvents = interactive ? "" : "none";
        }
        function updateControls() {
            dots.forEach((dot, index) => {
                if (index === active) dot.setAttribute("aria-current", "true");
                else dot.removeAttribute("aria-current");
            });
            if (counter) counter.textContent = hero ? (active + 1) + " / " + items.length :
                (active * perPage + 1) + "–" + Math.min((active + 1) * perPage, items.length) + " / " + items.length;
            if (controls) controls.hidden = pages() <= 1;
        }
        function settle() {
            const visible = members(active);
            items.forEach(item => {
                state(item, visible.includes(item));
                item.style.zIndex = "";
            });
            carousel.removeAttribute("data-transitioning");
            updateControls();
        }
        function measure() {
            // The hero's overlapping CSS grid reserves natural height before the first paint.
            if (hero) return;
            // Measure natural content at the current column width, never a guessed max-height.
            const flags = items.map(item => item.hidden);
            carousel.classList.add("lp-carousel-measuring");
            items.forEach((item, index) => {
                item.style.setProperty("--lp-slide-column", String(index % perPage + 1));
                item.hidden = false;
            });
            const height = Math.ceil(Math.max(...items.map(item => item.getBoundingClientRect().height)));
            items.forEach((item, index) => { item.hidden = flags[index]; });
            carousel.classList.remove("lp-carousel-measuring");
            track.style.height = height + "px";
        }
        async function decode(visible) {
            await Promise.all(visible.map(async item => {
                const img = item.querySelector("img");
                if (img) loadImage(img);
                if (!img || (img.complete && img.naturalWidth)) return;
                img.loading = "eager";
                try { await img.decode(); }
                catch {
                    if (fallback && !img.naturalWidth) {
                        img.removeAttribute("srcset");
                        img.src = fallback;
                        try { await img.decode(); } catch { /* Readable background remains. */ }
                    }
                }
            }));
        }
        function warmNext() {
            if (!hero || items.length < 2 || document.readyState !== "complete") return;
            const img = items[(active + 1) % items.length].querySelector("img");
            if (img) { img.loading = "eager"; loadImage(img); }
        }
        function loadImage(img) {
            if (!img.dataset.slideSrc) return;
            if (img.dataset.slideSrcset) img.srcset = img.dataset.slideSrcset;
            img.src = img.dataset.slideSrc;
            delete img.dataset.slideSrc;
            delete img.dataset.slideSrcset;
        }
        async function pump() {
            if (busy || desired === active) return;
            busy = true;
            loading = true;
            const ticket = ++generation;
            const target = desired;
            const incoming = members(target);
            await decode(incoming);
            if (ticket !== generation) return;
            loading = false;
            // Coalesce rapid input while loading; an already visible image remains untouched.
            if (target !== desired) { busy = false; pump(); return; }
            if (items.some(item => item.contains(document.activeElement) && !incoming.includes(item))) {
                desired = active;
                busy = false;
                return;
            }
            const outgoing = members(active);
            const direction = target >= active ? 1 : -1;
            active = target;
            if (reduced.matches || !motion || document.hidden) {
                settle(); busy = false; warmNext(); pump(); return;
            }
            carousel.dataset.transitioning = "";
            outgoing.forEach(item => { state(item, true, false); item.style.zIndex = "1"; });
            incoming.forEach(item => { state(item, true); item.style.zIndex = "2"; });
            updateControls();
            const milliseconds = motion.duration(hero ? "hero" : "reviews");
            animations = [];
            if (hero) {
                // Incoming opaque slide fades over the still-visible outgoing photograph.
                incoming.forEach(item => {
                    animations.push(animate(item, [{ opacity: 0 }, { opacity: 1 }], milliseconds));
                    const copy = item.querySelector(".lp-banner-copy");
                    if (copy) animations.push(animate(copy, [
                        { opacity: 0, transform: "translateY(8px)" },
                        { opacity: 1, transform: "translateY(0)" }
                    ], milliseconds - 160, 160));
                });
                outgoing.forEach(item => {
                    const copy = item.querySelector(".lp-banner-copy");
                    if (copy) animations.push(animate(copy, [{ opacity: 1 }, { opacity: 0 }], milliseconds));
                });
            } else {
                outgoing.forEach(item => animations.push(animate(item, [
                    { opacity: 1, transform: "translateX(0)" },
                    { opacity: 0, transform: "translateX(" + (-direction * 12) + "px)" }
                ], milliseconds)));
                incoming.forEach(item => animations.push(animate(item, [
                    { opacity: 0, transform: "translateX(" + (direction * 12) + "px)" },
                    { opacity: 1, transform: "translateX(0)" }
                ], milliseconds)));
            }
            await Promise.all(animations.map(a => a.finished));
            if (ticket !== generation) return;
            settle(); busy = false; warmNext(); pump();
        }
        function request(index) {
            desired = wrap(index);
            // A stalled image must not prevent choosing another already loaded slide.
            if (loading) { generation++; loading = false; busy = false; }
            pump();
        }
        const move = delta => request(desired + delta);
        carousel.querySelector("[data-carousel-prev]")?.addEventListener("click", () => move(-1));
        carousel.querySelector("[data-carousel-next]")?.addEventListener("click", () => move(1));
        dots.forEach((dot, index) => dot.addEventListener("click", () => request(index)));
        controls?.addEventListener("keydown", event => {
            if (event.key !== "ArrowLeft" && event.key !== "ArrowRight") return;
            event.preventDefault();
            move(event.key === "ArrowRight" ? 1 : -1);
        });
        let startTouch;
        track.addEventListener("pointerdown", event => {
            startTouch = event.pointerType === "touch" && !event.target.closest("a,button,input,textarea,summary")
                ? { x: event.clientX, y: event.clientY } : null;
        }, { passive: true });
        track.addEventListener("pointerup", event => {
            if (!startTouch) return;
            const dx = event.clientX - startTouch.x, dy = event.clientY - startTouch.y;
            startTouch = null;
            if (Math.abs(dx) > 50 && Math.abs(dx) > Math.abs(dy) * 1.5) move(dx < 0 ? 1 : -1);
        }, { passive: true });
        track.addEventListener("pointercancel", () => { startTouch = null; });
        function layout() {
            const focusedIndex = items.findIndex(item => item.contains(document.activeElement));
            const leadingIndex = focusedIndex >= 0 ? focusedIndex : active * perPage;
            generation++;
            animations.forEach(a => a.cancel());
            busy = false;
            loading = false;
            perPage = capacity();
            active = Math.min(Math.floor(leadingIndex / perPage), pages() - 1);
            desired = active;
            measure(); settle(); warmNext();
        }
        reduced.addEventListener("change", layout);
        document.addEventListener("visibilitychange", () => { if (document.hidden) layout(); });
        let layoutFrame;
        const scheduleLayout = () => {
            cancelAnimationFrame(layoutFrame);
            layoutFrame = requestAnimationFrame(layout);
        };
        if (window.ResizeObserver) {
            let width = track.getBoundingClientRect().width;
            new ResizeObserver(entries => {
                const nextWidth = entries[0].contentRect.width;
                if (Math.abs(nextWidth - width) > .5) { width = nextWidth; scheduleLayout(); }
            }).observe(track);
        } else window.addEventListener("resize", scheduleLayout);
        new MutationObserver(scheduleLayout).observe(track, { childList: true, characterData: true, subtree: true });
        document.fonts?.ready.then(scheduleLayout);
        if (hero) window.addEventListener("load", warmNext, { once: true });
        layout();
    });
})();
