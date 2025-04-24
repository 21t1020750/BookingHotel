const toggle = document.getElementById("menu-toggle");
const mobileMenu = document.getElementById("mobile-menu");
const desktopNav = document.getElementById("desktop-nav");
const menuIcon = document.getElementById("menu-icon");

let isOpen = false;

// Toggle mobile menu visibility and icon
toggle.addEventListener("click", () => {
    isOpen = !isOpen;
    if (isOpen) {
        mobileMenu.classList.remove("max-h-0");
        mobileMenu.classList.add("max-h-[500px]");
        menuIcon.textContent = "✖";
    } else {
        mobileMenu.classList.add("max-h-0");
        mobileMenu.classList.remove("max-h-[500px]");
        menuIcon.textContent = "☰";
    }
});

// Handle resize: Show desktop nav on large screens, hide mobile menu
window.addEventListener("resize", () => {
    if (window.innerWidth >= 768) {
        // Show desktop nav, hide mobile menu
        desktopNav.classList.remove("hidden");
        mobileMenu.classList.add("max-h-0");
        mobileMenu.classList.remove("max-h-[500px]");
        isOpen = false;
        menuIcon.textContent = "☰";
    } else {
        // Hide desktop nav on mobile
        desktopNav.classList.add("hidden");
    }
});

// Initial check for screen size on page load
if (window.innerWidth >= 768) {
    desktopNav.classList.remove("hidden");
    mobileMenu.classList.add("max-h-0");
} else {
    desktopNav.classList.add("hidden");
}