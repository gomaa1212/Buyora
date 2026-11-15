// Buyora - Modern Interactive & Responsive Frontend Experience

document.addEventListener("DOMContentLoaded", function () {
    initBackToTop();
    initSmoothAnchors();
    initImageFadeIn();
    initAlertAutoDismiss();
    initMobileNavigation();
});

// 1. Copy Tracking Number
document.addEventListener("click", async function (event) {
    const button = event.target.closest("[data-copy-tracking-number]");
    if (!button) return;

    const trackingNumber = button.dataset.copyTrackingNumber;
    let copied = false;

    if (navigator.clipboard && window.isSecureContext) {
        try {
            await navigator.clipboard.writeText(trackingNumber);
            copied = true;
        } catch {
            // Fall back for browsers or contexts that block the Clipboard API.
        }
    }

    if (!copied) {
        const temporaryInput = document.createElement("textarea");
        temporaryInput.value = trackingNumber;
        temporaryInput.setAttribute("readonly", "");
        temporaryInput.style.position = "fixed";
        temporaryInput.style.opacity = "0";
        document.body.appendChild(temporaryInput);
        temporaryInput.select();
        try {
            copied = document.execCommand("copy");
        } finally {
            temporaryInput.remove();
        }
    }

    if (copied && typeof toastr !== "undefined") {
        const msg = button.dataset.copySuccessMessage 
            ? button.dataset.copySuccessMessage.replace("{0}", trackingNumber)
            : "Tracking number copied!";
        toastr.success(msg);
    } else if (!copied && typeof toastr !== "undefined") {
        toastr.error("Could not copy the tracking number.");
    }
});

// 2. Back To Top Floating Action Button
function initBackToTop() {
    const backToTopBtn = document.getElementById("buyoraBackToTop");
    if (!backToTopBtn) return;

    let ticking = false;
    window.addEventListener("scroll", function () {
        if (!ticking) {
            window.requestAnimationFrame(function () {
                if (window.scrollY > 280) {
                    backToTopBtn.classList.add("show");
                } else {
                    backToTopBtn.classList.remove("show");
                }
                ticking = false;
            });
            ticking = true;
        }
    }, { passive: true });

    backToTopBtn.addEventListener("click", function () {
        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    });
}

// 3. Smooth scrolling for on-page hash anchors
function initSmoothAnchors() {
    document.addEventListener("click", function (e) {
        const anchor = e.target.closest('a[href^="#"]:not([href="#"]):not([data-bs-toggle])');
        if (!anchor) return;

        const targetId = anchor.getAttribute("href");
        if (!targetId || targetId === "#") return;

        const targetEl = document.querySelector(targetId);
        if (targetEl) {
            e.preventDefault();
            const headerOffset = 80;
            const elementPosition = targetEl.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

            window.scrollTo({
                top: offsetPosition,
                behavior: "smooth"
            });
        }
    });
}

// 4. Graceful image loading fade-in
function initImageFadeIn() {
    const images = document.querySelectorAll(".buyora-product-img, .buyora-stage-img, .buyora-cart-item-img");
    images.forEach(function (img) {
        if (img.complete) {
            img.classList.add("img-loaded");
        } else {
            img.addEventListener("load", function () {
                img.classList.add("img-loaded");
            });
        }
    });
}

// 5. Auto dismiss non-error alerts after 6 seconds
function initAlertAutoDismiss() {
    const alerts = document.querySelectorAll(".alert.alert-success, .alert.alert-info");
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.style.transition = "opacity 0.4s ease, transform 0.4s ease";
            alert.style.opacity = "0";
            alert.style.transform = "translateY(-8px)";
            setTimeout(function () {
                alert.remove();
            }, 400);
        }, 5500);
    });
}

// 6. Mobile bottom navigation enhancements
function initMobileNavigation() {
    // When user profile icon in bottom nav is clicked, toggle navbar and scroll to top smoothly
    document.addEventListener("click", function (e) {
        const toggleBtn = e.target.closest(".js-mobile-user-toggle");
        if (toggleBtn) {
            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        }

        // Make the entire bottom nav cart item clickable
        const cartItem = e.target.closest(".buyora-bottom-nav-cart");
        if (cartItem && !e.target.closest("a")) {
            const link = cartItem.querySelector("a");
            if (link) {
                link.click();
            }
        }
    });
}
