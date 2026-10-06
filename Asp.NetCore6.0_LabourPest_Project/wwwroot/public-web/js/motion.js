(() => {
    "use strict";
    const root = document.querySelector(".lp-public");
    if (!root) return;
    const reduced = matchMedia("(prefers-reduced-motion: reduce)");
    const styles = getComputedStyle(root);
    const duration = name => parseFloat(styles.getPropertyValue("--lp-motion-" + name)) || 0;
    const easing = styles.getPropertyValue("--lp-motion-ease").trim() || "ease";
    // WAAPI completion plus a timer: interrupted animations never leave a component locked.
    function animate(element, frames, milliseconds, delay = 0) {
        if (reduced.matches || !element.animate || !milliseconds) return { finished: Promise.resolve(), cancel() {} };
        let animation, timer, resolve;
        let done = false;
        const finished = new Promise(r => { resolve = r; });
        const complete = () => {
            if (done) return;
            done = true;
            clearTimeout(timer);
            animation?.cancel();
            resolve();
        };
        try {
            animation = element.animate(frames, { duration: milliseconds, delay, easing, fill: "both" });
            animation.finished.then(complete, complete);
            timer = setTimeout(complete, milliseconds + delay + 180);
        } catch { complete(); }
        return { finished, cancel: complete };
    }
    window.LabourPestMotion = { reduced, duration, animate };

    root.querySelectorAll("#ss .lp-disclosure").forEach(details => {
        const summary = details.querySelector("summary");
        const panel = summary?.nextElementSibling;
        if (!summary || !panel || !details.animate) return;
        let desired = details.open, generation = 0, running = false, animations = [];
        function finish() {
            details.open = desired;
            summary.setAttribute("aria-expanded", String(desired));
            details.style.height = "";
            details.style.overflow = "";
            details.removeAttribute("data-closing");
            details.removeAttribute("data-animating");
            running = false;
        }
        async function toggle(open) {
            const ticket = ++generation;
            const startHeight = details.getBoundingClientRect().height;
            const startOpacity = details.open ? getComputedStyle(panel).opacity : "0";
            animations.forEach(a => a.cancel());
            desired = open;
            if (reduced.matches) { finish(); return; }
            running = true;
            details.dataset.animating = "";
            details.toggleAttribute("data-closing", !open);
            details.style.height = startHeight + "px";
            details.style.overflow = "hidden";
            // Closing remains semantically open until its visible content finishes collapsing.
            details.open = true;
            summary.setAttribute("aria-expanded", "true");
            const css = getComputedStyle(details);
            const endHeight = summary.getBoundingClientRect().height +
                (open ? panel.getBoundingClientRect().height : 0) +
                parseFloat(css.borderTopWidth) + parseFloat(css.borderBottomWidth);
            animations = [
                animate(details, [{ height: startHeight + "px" }, { height: endHeight + "px" }], duration("accordion")),
                animate(panel, [{ opacity: startOpacity }, { opacity: open ? 1 : 0 }], duration("accordion"))
            ];
            await Promise.all(animations.map(a => a.finished));
            if (ticket === generation) finish();
        }
        summary.addEventListener("click", event => {
            event.preventDefault();
            toggle(!desired);
        });
        details.addEventListener("toggle", () => {
            // Native hash/find actions and script-free behavior do not depend on the animator.
            if (!running) desired = details.open;
        });
        reduced.addEventListener("change", () => {
            if (!reduced.matches) return;
            generation++;
            animations.forEach(a => a.cancel());
            finish();
        });
        if (window.ResizeObserver) {
            let previous = "";
            const observer = new ResizeObserver(entries => {
                const key = entries.map(e => e.contentRect.width + ":" + e.contentRect.height).join("|");
                if (key !== previous && running) toggle(desired);
                previous = key;
            });
            observer.observe(panel);
            observer.observe(summary);
        }
    });

    // Readable by default; only below-the-fold headings receive a one-time enhancement.
    if (window.IntersectionObserver && !reduced.matches && !location.hash) {
        const entrances = new Set();
        const observer = new IntersectionObserver(entries => entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            observer.unobserve(entry.target);
            if (reduced.matches || entry.target.contains(document.activeElement)) return;
            const animation = animate(entry.target, [
                { opacity: .65, transform: "translateY(12px)" },
                { opacity: 1, transform: "translateY(0)" }
            ], duration("entrance"));
            entrances.add(animation);
            animation.finished.then(() => entrances.delete(animation));
        }), { threshold: .15 });
        root.querySelectorAll("main .lp-section-heading").forEach(heading => {
            if (heading.getBoundingClientRect().top >= innerHeight) observer.observe(heading);
        });
        reduced.addEventListener("change", () => {
            if (reduced.matches) { observer.disconnect(); entrances.forEach(a => a.cancel()); }
        });
    }
})();
