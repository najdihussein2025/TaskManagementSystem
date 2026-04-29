document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("categoryForm");
  const title = document.getElementById("catFormTitle");
  const cancel = document.getElementById("cancelEdit");
  const picker = document.getElementById("colorPicker");

  picker?.querySelectorAll("button").forEach((btn) => btn.addEventListener("click", () => {
    picker.querySelectorAll("button").forEach((b) => b.classList.remove("selected"));
    btn.classList.add("selected");
  }));

  const resetForm = () => {
    form?.reset();
    form.classList.remove("editing");
    title.textContent = "Add New Category";
    cancel.style.display = "none";
  };

  document.querySelectorAll(".edit-cat").forEach((btn) => btn.addEventListener("click", () => {
    title.textContent = "Edit Category";
    form.classList.add("editing");
    cancel.style.display = "inline-block";
  }));

  cancel?.addEventListener("click", resetForm);
  form?.addEventListener("submit", () => resetForm());

  document.querySelectorAll(".delete-cat, .confirm-delete .no, .confirm-delete .yes").forEach((btn) =>
    btn.addEventListener("click", () => {})
  );
});
