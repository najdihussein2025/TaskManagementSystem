document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("categoryForm");
  const title = document.getElementById("catFormTitle");
  const cancel = document.getElementById("cancelEdit");
  const picker = document.getElementById("colorPicker");
  const idInput = document.getElementById("catId");
  const nameInput = document.getElementById("catName");
  const descInput = document.getElementById("catDesc");
  const colorInput = document.getElementById("catColor");
  const quickAddBtn = document.getElementById("catQuickAdd");

  const updateSelectedColor = (color) => {
    if (colorInput) colorInput.value = color;
    picker?.querySelectorAll("button").forEach((button) => {
      button.classList.toggle("selected", button.dataset.color === color);
    });
  };

  picker?.querySelectorAll("button").forEach((btn) => btn.addEventListener("click", () => {
    updateSelectedColor(btn.dataset.color || "#3b82f6");
  }));

  const resetForm = () => {
    form?.reset();
    if (form) form.action = "/Category/Create";
    if (idInput) idInput.value = "";
    form?.classList.remove("editing");
    if (title) title.textContent = "Add New Category";
    if (cancel) cancel.style.display = "none";
    updateSelectedColor("#3b82f6");
  };

  document.querySelectorAll(".edit-cat").forEach((btn) => btn.addEventListener("click", () => {
    const row = btn.closest("tr");
    if (!row || !form) return;

    if (idInput) idInput.value = row.dataset.id || "";
    if (nameInput) nameInput.value = row.dataset.name || "";
    if (descInput) descInput.value = row.dataset.desc || "";
    updateSelectedColor(row.dataset.color || "#3b82f6");

    form.action = "/Category/Edit";
    form.classList.add("editing");
    if (title) title.textContent = "Edit Category";
    if (cancel) cancel.style.display = "inline-block";
  }));

  quickAddBtn?.addEventListener("click", () => {
    resetForm();
    form?.scrollIntoView({ behavior: "smooth", block: "center" });
  });
  cancel?.addEventListener("click", resetForm);
});
