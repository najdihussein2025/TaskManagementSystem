document.addEventListener("DOMContentLoaded", () => {
  animateCounters();
  animateStatusBars();
});

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

function animateStatusBars() {
  const bars = document.querySelectorAll(".bar-fill[data-width]");
  bars.forEach((bar) => {
    const target = bar.getAttribute("data-width");
    requestAnimationFrame(() => {
      bar.style.width = target;
    });
  });
}
