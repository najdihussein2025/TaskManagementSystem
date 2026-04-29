document.addEventListener("DOMContentLoaded", () => {
  const wrapper = document.getElementById("userWrapper");
  const sidebar = document.getElementById("userSidebar");
  const collapseBtn = document.getElementById("collapseSidebar");
  const mobileMenuBtn = document.getElementById("mobileMenuBtn");
  const notifBtn = document.getElementById("notifBtn");
  const notifMenu = document.getElementById("notifMenu");
  const avatarBtn = document.getElementById("avatarBtn");
  const profileMenu = document.getElementById("profileMenu");

  setTodayDate();
  setupNavActiveState();

  collapseBtn?.addEventListener("click", () => {
    sidebar?.classList.toggle("collapsed");
  });

  mobileMenuBtn?.addEventListener("click", () => {
    wrapper?.classList.toggle("sidebar-open");
  });

  notifBtn?.addEventListener("click", (event) => {
    event.stopPropagation();
    notifMenu?.classList.toggle("open");
    profileMenu?.classList.remove("open");
  });

  avatarBtn?.addEventListener("click", (event) => {
    event.stopPropagation();
    profileMenu?.classList.toggle("open");
    notifMenu?.classList.remove("open");
  });

  document.addEventListener("click", (event) => {
    if (!event.target.closest(".dropdown-wrap")) {
      notifMenu?.classList.remove("open");
      profileMenu?.classList.remove("open");
    }

    if (
      window.innerWidth < 768 &&
      !event.target.closest("#userSidebar") &&
      !event.target.closest("#mobileMenuBtn")
    ) {
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

function setupNavActiveState() {
  const navItems = document.querySelectorAll("#sideNav .nav-item");
  navItems.forEach((item) => {
    item.addEventListener("click", () => {
      navItems.forEach((navItem) => navItem.classList.remove("active"));
      item.classList.add("active");
    });
  });
}
