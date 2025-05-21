document.addEventListener("DOMContentLoaded", function () {
    // Menu Toggle Elements
    const toggle = document.getElementById("menu-toggle");
    const mobileMenu = document.getElementById("mobile-menu");
    const desktopNav = document.getElementById("desktop-nav");
    const desktopAuth = document.getElementById("desktop-auth");
    const menuIcon = document.getElementById("menu-icon");

    // Language & User Dropdown Elements
    const langToggle = document.getElementById("lang-toggle");
    const langMenu = document.getElementById("lang-menu");
    const currentLang = document.getElementById("current-lang");
    const userDropdown = document.getElementById("userDropdown");
    const dropdownMenu = document.getElementById("dropdownMenu");

    // Navigation Links
    const navLinks = document.querySelectorAll(".nav-link");
    const header = document.querySelector('header');

    let isOpen = false;

    // Toggle mobile menu visibility and icon
    if (toggle) {
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
    }

    // Function to handle visibility based on screen size
    function handleResize() {
        if (window.innerWidth >= 1024) {
            // Desktop view: Show desktop nav and auth, hide mobile menu
            desktopNav?.classList.remove("hidden");
            desktopAuth?.classList.remove("hidden");
            mobileMenu?.classList.add("max-h-0");
            mobileMenu?.classList.remove("max-h-[500px]");
            isOpen = false;
            if (menuIcon) menuIcon.textContent = "☰";
        } else {
            // Mobile view: Hide desktop nav and auth
            desktopNav?.classList.add("hidden");
            desktopAuth?.classList.add("hidden");
        }
    }

    // Handle resize
    window.addEventListener("resize", handleResize);

    // Initial check for screen size on page load
    handleResize();

    // Handle active link state
    navLinks.forEach(link => {
        link.addEventListener("click", () => {
            // Remove active class from all links
            navLinks.forEach(l => l.classList.remove("active"));
            // Add active class to clicked link
            link.classList.add("active");
        });
    });

    // Scroll effect for header
    window.addEventListener('scroll', function () {
        if (window.scrollY > 10) {
            header.classList.add('py-2');
            header.classList.add('shadow-lg');
            header.classList.add('bg-gradient-to-r', 'from-blue-700', 'to-cyan-600');
            header.classList.remove('from-cyan-600', 'to-blue-700');
        } else {
            header.classList.remove('py-2');
            header.classList.remove('shadow-lg');
            header.classList.remove('from-blue-700', 'to-cyan-600');
            header.classList.add('from-cyan-600', 'to-blue-700');
        }
    });

    // Language dropdown handling
    if (langToggle) {
        langToggle.addEventListener("click", function (event) {
            event.stopPropagation();
            langMenu.classList.toggle("hidden");
        });
    }

    // Language selection handling
    if (langMenu) {
        langMenu.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function (event) {
                event.preventDefault();
                const selectedLang = this.getAttribute("data-lang");
                if (currentLang) currentLang.textContent = selectedLang;
                window.location.href = this.getAttribute("href");
            });
        });
    }

    // User dropdown handling
    if (userDropdown) {
        userDropdown.addEventListener("click", function (event) {
            event.stopPropagation();
            if (dropdownMenu) {
                dropdownMenu.classList.toggle("hidden");
                if (!dropdownMenu.classList.contains("hidden")) {
                    // Animation for dropdown
                    dropdownMenu.classList.add("scale-100", "opacity-100");
                    dropdownMenu.classList.remove("scale-95", "opacity-0");
                } else {
                    // Animation for dropdown hiding
                    dropdownMenu.classList.remove("scale-100", "opacity-100");
                    dropdownMenu.classList.add("scale-95", "opacity-0");
                }
            }
        });
    }

    // Close dropdowns when clicking outside
    document.addEventListener("click", function () {
        if (dropdownMenu) dropdownMenu.classList.add("hidden");
        if (langMenu) langMenu.classList.add("hidden");
    });

    // Close dropdowns with Escape key
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") {
            if (dropdownMenu) dropdownMenu.classList.add("hidden");
            if (langMenu) langMenu.classList.add("hidden");
        }
    });

    // Highlight current page link based on URL path
    function highlightCurrentPage() {
        const currentPath = window.location.pathname.toLowerCase();
        navLinks.forEach(link => {
            const href = link.getAttribute('href') || link.querySelector('a')?.getAttribute('href') || '';
            const linkPath = href.toLowerCase();

            if ((currentPath === '/' && (linkPath === '/' || linkPath === '~/')) ||
                (currentPath !== '/' && linkPath !== '/' && currentPath.includes(linkPath.replace('~/', '')))) {
                link.classList.add('active');
            }
        });
    }

    // Call once on page load
    highlightCurrentPage();
});

// Tab switching function for user profile page
function switchTab(tabId) {
    // If we're on the Information/Index page
    if (window.location.pathname.includes('/Information/Index')) {
        // Try to use the page's own activateTab function if it exists
        if (typeof activateTab === 'function') {
            activateTab(tabId);
        } else {
            // Manual fallback for tab switching
            const sections = ['profile', 'bookings', 'history', 'reviews', 'security', 'rewards'];
            const tabs = document.querySelectorAll('.nav-tab');

            if (sections.includes(tabId)) {
                // Update tab active states
                tabs.forEach(t => t.classList.remove('active'));
                const activeTab = Array.from(tabs).find(tab => tab.getAttribute('href').substring(1) === tabId);
                if (activeTab) {
                    activeTab.classList.add('active');
                }

                // Show/hide content sections
                sections.forEach(section => {
                    const element = document.getElementById(section);
                    if (element) {
                        element.classList.toggle('hidden', section !== tabId);
                    }
                });
            }
        }

        // Update URL hash if needed
        const currentHash = window.location.hash.substring(1);
        if (currentHash !== tabId) {
            window.location.hash = tabId;
        } else {
            // Trigger hashchange to handle the case when clicking the same tab twice
            window.dispatchEvent(new HashChangeEvent("hashchange"));
        }
    } else {
        // If on a different page, navigate to the profile page with the correct hash
        window.location.href = `/Information/Index#${tabId}`;
    }
}