document.addEventListener("DOMContentLoaded", () => {
  const wrapper = document.getElementById("adminWrapper");
  const sidebar = document.getElementById("adminSidebar");
  const collapseBtn = document.getElementById("collapseSidebar");
  const mobileMenuBtn = document.getElementById("mobileMenuBtn");
  const notifBtn = document.getElementById("notifBtn");
  const avatarBtn = document.getElementById("avatarBtn");
  const notifMenu = document.getElementById("notifMenu");
  const profileMenu = document.getElementById("profileMenu");

  if (collapseBtn && sidebar) {
    collapseBtn.addEventListener("click", () => {
      sidebar.classList.toggle("collapsed");
    });
  }

  if (mobileMenuBtn && wrapper) {
    mobileMenuBtn.addEventListener("click", () => {
      wrapper.classList.toggle("sidebar-open");
    });
  }

  setTodayDate();
  animateStatusBars();
  animateCounters();
  setupNavActiveState();

  if (notifBtn && notifMenu) {
    notifBtn.addEventListener("click", (event) => {
      event.stopPropagation();
      notifMenu.classList.toggle("open");
      profileMenu?.classList.remove("open");
    });
  }

  if (avatarBtn && profileMenu) {
    avatarBtn.addEventListener("click", (event) => {
      event.stopPropagation();
      profileMenu.classList.toggle("open");
      notifMenu?.classList.remove("open");
    });
  }

  document.addEventListener("click", (event) => {
    if (!event.target.closest(".dropdown-wrap")) {
      notifMenu?.classList.remove("open");
      profileMenu?.classList.remove("open");
    }
    if (window.innerWidth < 768 && !event.target.closest("#adminSidebar") && !event.target.closest("#mobileMenuBtn")) {
      wrapper?.classList.remove("sidebar-open");
    }
  });
});

function setTodayDate() {
  const dateElement = document.getElementById("todayDate");
  if (!dateElement) return;

  const date = new Date();
  dateElement.textContent = date.toLocaleDateString(undefined, {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric"
  });
}

function animateStatusBars() {
  const bars = document.querySelectorAll(".bar-fill[data-width]");
  bars.forEach((bar) => {
    const target = bar.getAttribute("data-width");
    requestAnimationFrame(() => {
      bar.style.width = target;
    });
  });
}

function animateCounters() {
  const counters = document.querySelectorAll("[data-counter]");
  const duration = 1000;

  counters.forEach((counterEl) => {
    const target = Number(counterEl.getAttribute("data-counter"));
    const startTime = performance.now();

    const update = (time) => {
      const progress = Math.min((time - startTime) / duration, 1);
      counterEl.textContent = Math.floor(progress * target).toString();
      if (progress < 1) {
        requestAnimationFrame(update);
      } else {
        counterEl.textContent = target.toString();
      }
    };

    requestAnimationFrame(update);
  });
}

function setupNavActiveState() {
  const navItems = document.querySelectorAll("#sideNav .nav-item");
  navItems.forEach((item) => {
    item.addEventListener("click", () => {
      navItems.forEach((navItem) => navItem.classList.remove("active"));
      item.classList.add("active");
    });
  });
}
