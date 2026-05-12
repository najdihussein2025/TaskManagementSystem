document.addEventListener("DOMContentLoaded", () => {
  const line = document.getElementById("reportPolyline");

  line?.classList.add("animate");
  document.querySelectorAll(".bar-fill[data-width]").forEach((bar) => {
    requestAnimationFrame(() => { bar.style.width = bar.dataset.width; });
  });
});
