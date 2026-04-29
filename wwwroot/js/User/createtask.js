document.addEventListener("DOMContentLoaded", () => {
  const descriptionInput = document.getElementById("taskDescription");
  const counter = document.getElementById("charCounter");

  if (!descriptionInput || !counter) return;

  const updateCount = () => {
    counter.textContent = `${descriptionInput.value.length} / 500`;
  };

  descriptionInput.addEventListener("input", updateCount);
  updateCount();
});
