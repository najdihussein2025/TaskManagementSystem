document.addEventListener("DOMContentLoaded", () => {
  const dirtyNotice = document.getElementById("dirtyNotice");
  const toast = document.getElementById("settingsToast");
  const saveBtn = document.getElementById("saveSettings");

  document.querySelectorAll("[data-toggle]").forEach((t) => t.addEventListener("click", () => {
    t.classList.toggle("on");
    dirtyNotice?.classList.add("show");
  }));

  document.querySelectorAll("[data-dirty]").forEach((el) => el.addEventListener("input", () => dirtyNotice?.classList.add("show")));

  document.querySelectorAll(".danger-btn").forEach((btn) => btn.addEventListener("click", () => {
    const box = btn.nextElementSibling;
    box.classList.toggle("show");
  }));
  document.querySelectorAll(".danger-confirm button:last-child").forEach((btn) => btn.addEventListener("click", () => btn.closest(".danger-confirm").classList.remove("show")));

  saveBtn?.addEventListener("click", () => {
    toast.classList.add("show");
    dirtyNotice?.classList.remove("show");
    setTimeout(() => toast.classList.remove("show"), 3000);
  });

  document.querySelectorAll(".pw-toggle").forEach((btn) => btn.addEventListener("click", () => {
    const input = btn.previousElementSibling;
    input.type = input.type === "password" ? "text" : "password";
  }));
});
