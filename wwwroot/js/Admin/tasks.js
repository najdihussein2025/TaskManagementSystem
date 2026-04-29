document.addEventListener("DOMContentLoaded", () => {
  const formOverlay = document.getElementById("taskFormOverlay");
  const detailOverlay = document.getElementById("taskDetailOverlay");
  const openBtn = document.getElementById("openTaskModal");
  const cancelBtn = document.getElementById("cancelTaskModal");
  const closeDetail = document.getElementById("closeTaskDetail");
  const form = document.getElementById("taskForm");

  const openForm = () => {
    formOverlay.classList.add("open");
  };
  const closeForm = () => {
    formOverlay.classList.remove("open");
    form?.reset();
  };

  openBtn?.addEventListener("click", openForm);
  cancelBtn?.addEventListener("click", closeForm);
  formOverlay?.addEventListener("click", (e) => { if (e.target === formOverlay) closeForm(); });
  detailOverlay?.addEventListener("click", (e) => { if (e.target === detailOverlay) detailOverlay.classList.remove("open"); });
  closeDetail?.addEventListener("click", () => detailOverlay.classList.remove("open"));
  form?.addEventListener("submit", () => closeForm());

  document.querySelectorAll(".edit-task").forEach((btn) =>
    btn.addEventListener("click", () => openForm())
  );

  document.querySelectorAll(".view-task").forEach((btn) =>
    btn.addEventListener("click", () => detailOverlay?.classList.add("open"))
  );

  document.querySelectorAll(".delete-task, .confirm-delete .no, .confirm-delete .yes").forEach((btn) =>
    btn.addEventListener("click", () => {})
  );
});
