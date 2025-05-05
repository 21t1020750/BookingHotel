const toggle = document.getElementById("menu-toggle");
const mobileMenu = document.getElementById("mobile-menu");
const desktopNav = document.getElementById("desktop-nav");
const desktopAuth = document.getElementById("desktop-auth");
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

// Function to handle visibility based on screen size
function handleResize() {
    if (window.innerWidth >= 1024) {
        // Desktop view: Show desktop nav and auth, hide mobile menu
        desktopNav.classList.remove("hidden");
        desktopAuth.classList.remove("hidden");
        mobileMenu.classList.add("max-h-0");
        mobileMenu.classList.remove("max-h-[500px]");
        isOpen = false;
        menuIcon.textContent = "☰";
    } else {
        // Desktop view: Hide desktop nav and auth
        desktopNav.classList.add("hidden");
        desktopAuth.classList.add("hidden");
    }
}

// Handle resize
window.addEventListener("resize", handleResize);

// Initial check for screen size on page load
handleResize();

// Handle active link state
const navLinks = document.querySelectorAll(".nav-link");
navLinks.forEach(link => {
    link.addEventListener("click", () => {
        // Remove active class from all links
        navLinks.forEach(l => l.classList.remove("active"));
        // Add active class to clicked link
        link.classList.add("active");
    });
});