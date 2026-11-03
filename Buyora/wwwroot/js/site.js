// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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
        toastr.success(button.dataset.copySuccessMessage.replace("{0}", trackingNumber));
    } else if (!copied && typeof toastr !== "undefined") {
        toastr.error("Could not copy the tracking number.");
    }
});
