// common.js — Shared UI utilities for InventoryManagement
// Include this on any page that uses shared components (scroll-to-top, etc.)

// =============================================
// SCROLL TO TOP
// =============================================
const ScrollToTop = (() => {
    const SHOW_THRESHOLD = 300; // px scrolled before button appears

    function init(buttonId = 'scrollToTopBtn') {
        const btn = document.getElementById(buttonId);
        if (!btn) return;

        // Show / hide on scroll
        const scrollTarget = _getScrollTarget();
        scrollTarget.addEventListener('scroll', () => {
            const scrollY = scrollTarget === window
                ? window.scrollY
                : scrollTarget.scrollTop;

            btn.classList.toggle('stt-visible', scrollY > SHOW_THRESHOLD);
        });

        // Smooth scroll back to top
        btn.addEventListener('click', () => {
            if (scrollTarget === window) {
                window.scrollTo({ top: 0, behavior: 'smooth' });
            } else {
                scrollTarget.scrollTo({ top: 0, behavior: 'smooth' });
            }
        });
    }

    // Returns the element that actually scrolls on this page.
    // Falls back to window if no .main-content or .inv-main is found.
    function _getScrollTarget() {
        const candidates = [
            document.querySelector('.main-content'),
            document.querySelector('.inv-main'),
        ];
        for (const el of candidates) {
            if (el) return el;
        }
        return window;
    }

    return { init };
})();

// =============================================
// DOM-READY BOOTSTRAP
// =============================================
document.addEventListener('DOMContentLoaded', () => {
    ScrollToTop.init();
});
