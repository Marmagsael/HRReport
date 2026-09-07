/**
 * ==========================================================
 * Morpheusbox
 * Main JavaScript
 * ==========================================================
 */

document.addEventListener("DOMContentLoaded", () => {

    initMobileNavigation();
    initScrollSpy();
    initHeaderScroll();

});


/**
 * ==========================================================
 * Mobile Navigation
 * ==========================================================
 */

function initMobileNavigation() {

    const header = document.querySelector(".mbi-header");
    const navToggle = document.querySelector(".mbi-nav-toggle");
    const navLinks = document.querySelectorAll(".mbi-nav-link");

    if (!header || !navToggle) {
        return;
    }

    /*
    ----------------------------------------------------------
    Toggle Button
    ----------------------------------------------------------
    */

    navToggle.addEventListener("click", () => {

        toggleMobileMenu(header, navToggle);

    });

    /*
    ----------------------------------------------------------
    Navigation Links
    ----------------------------------------------------------
    */

    navLinks.forEach(link => {

        link.addEventListener("click", () => {

            setActiveNavigation(link);

            closeMobileMenu(header, navToggle);

        });

    });

    /*
    ----------------------------------------------------------
    ESC Key
    ----------------------------------------------------------
    */

    document.addEventListener("keydown", (event) => {

        if (event.key === "Escape") {

            closeMobileMenu(header, navToggle);

        }

    });

    /*
    ----------------------------------------------------------
    Click Outside Header
    ----------------------------------------------------------
    */

    document.addEventListener("click", (event) => {

        if (!header.contains(event.target)) {

            closeMobileMenu(header, navToggle);

        }

    });

    /*
    ----------------------------------------------------------
    Reset When Back To Desktop
    ----------------------------------------------------------
    */

    window.addEventListener("resize", () => {

        if (window.innerWidth >= 992) {

            closeMobileMenu(header, navToggle);

        }

    });

}


/**
 * ==========================================================
 * Scroll Spy
 * ==========================================================
 */

function initScrollSpy() {

    const sections = document.querySelectorAll("section[id]");

    const observer = new IntersectionObserver((entries) => {

        entries.forEach(entry => {

            if (!entry.isIntersecting) {
                return;
            }

            const id = entry.target.getAttribute("id");

            const activeLink = document.querySelector(
                `.mbi-nav-link[href="#${id}"]`
            );

            if (activeLink) {

                setActiveNavigation(activeLink);

            }

        });

    }, {

        threshold: 0.5

    });

    sections.forEach(section => observer.observe(section));

}


/**
 * ==========================================================
 * Header Scroll
 * ==========================================================
 */

function initHeaderScroll() {

    const header = document.querySelector(".mbi-header");

    if (!header) {
        return;
    }

    function updateHeader() {

        if (window.scrollY > 20) {

            header.classList.add("mbi-header-scrolled");

        } else {

            header.classList.remove("mbi-header-scrolled");

        }

    }

    updateHeader();

    window.addEventListener("scroll", updateHeader);

}

/**
 * ==========================================================
 * Update Navigation Icon
 * ==========================================================
 */

function updateNavigationIcon(button, isOpen) {

    const icon = button.querySelector("i");

    if (!icon) return;

    icon.classList.toggle("bi-list", !isOpen);
    icon.classList.toggle("bi-x", isOpen);

}


/**
 * ==========================================================
 * Open Mobile Menu
 * ==========================================================
 */

function openMobileMenu(header, navToggle) {

    header.classList.add("mbi-menu-open");

    navToggle.setAttribute("aria-expanded", "true");

    updateNavigationIcon(navToggle, true);

    document.body.classList.add("mbi-no-scroll");

}


/**
 * ==========================================================
 * Close Mobile Menu
 * ==========================================================
 */

function closeMobileMenu(header, navToggle) {

    header.classList.remove("mbi-menu-open");

    navToggle.setAttribute("aria-expanded", "false");

    updateNavigationIcon(navToggle, false);

    document.body.classList.remove("mbi-no-scroll");

}


/**
 * ==========================================================
 * Toggle Mobile Menu
 * ==========================================================
 */

function toggleMobileMenu(header, navToggle) {

    if (header.classList.contains("mbi-menu-open")) {

        closeMobileMenu(header, navToggle);

    } else {

        openMobileMenu(header, navToggle);

    }

}


/**
 * ==========================================================
 * Active Navigation
 * ==========================================================
 */

function setActiveNavigation(activeLink) {

    document.querySelectorAll(".mbi-nav-link").forEach(link => {

        link.classList.remove("mbi-active");

    });

    activeLink.classList.add("mbi-active");

}