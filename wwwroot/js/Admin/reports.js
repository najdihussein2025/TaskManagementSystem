document.addEventListener("DOMContentLoaded", () => {
  const exportBtn = document.getElementById("exportPdf");
  const toast = document.getElementById("reportToast");
  const line = document.getElementById("reportPolyline");

  line?.classList.add("animate");
  document.querySelectorAll(".bar-fill[data-width]").forEach((bar) => {
    requestAnimationFrame(() => { bar.style.width = bar.dataset.width; });
  });

  exportBtn?.addEventListener("click", () => {
    toast.classList.add("show");
    setTimeout(() => toast.classList.remove("show"), 3000);
  });
});
