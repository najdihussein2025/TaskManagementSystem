document.addEventListener("DOMContentLoaded", () => {
  const toggleButtons = document.querySelectorAll(".toggle-pass");
  const tabButtons = document.querySelectorAll(".tab-btn");

  toggleButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const inputId = button.getAttribute("data-target");
      const input = document.getElementById(inputId);
      if (!input) return;

      const isPassword = input.type === "password";
      input.type = isPassword ? "text" : "password";
      button.textContent = isPassword ? "Hide" : "Show";
    });
  });

  tabButtons.forEach((button) => {
    button.addEventListener("click", () => {
      tabButtons.forEach((item) => item.classList.remove("active"));
      button.classList.add("active");

      const targetId = button.getAttribute("data-tab");
      document.querySelectorAll(".section-panel").forEach((panel) => {
        panel.classList.remove("active");
      });
      document.getElementById(targetId)?.classList.add("active");
    });
  });
});
