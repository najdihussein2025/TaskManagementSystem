document.addEventListener("DOMContentLoaded", () => {
  const navbar = document.getElementById("mainNavbar");

  const handleNavbarState = () => {
    if (!navbar) {
      return;
    }
    navbar.classList.toggle("scrolled", window.scrollY > 16);
  };

  handleNavbarState();
  window.addEventListener("scroll", handleNavbarState, { passive: true });

  const smoothLinks = document.querySelectorAll('a[href^="#"]');
  smoothLinks.forEach((link) => {
    link.addEventListener("click", (event) => {
      const href = link.getAttribute("href");
      if (!href || href === "#") {
        return;
      }

      const target = document.querySelector(href);
      if (!target) {
        return;
      }

      event.preventDefault();
      target.scrollIntoView({ behavior: "smooth", block: "start" });
    });
  });
});
