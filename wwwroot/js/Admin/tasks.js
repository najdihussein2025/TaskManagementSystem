document.addEventListener("DOMContentLoaded", () => {
  const formOverlay = document.getElementById("taskFormOverlay");
  const detailOverlay = document.getElementById("taskDetailOverlay");
  const detailBox = document.getElementById("taskDetailBox");
  const openBtn = document.getElementById("openTaskModal");
  const cancelBtn = document.getElementById("cancelTaskModal");
  const closeDetail = document.getElementById("closeTaskDetail");
  const form = document.getElementById("taskForm");
  const modalTitle = document.getElementById("taskModalTitle");

  const tTitle = document.getElementById("tTitle");
  const tDesc = document.getElementById("tDesc");
  const tAssign = document.getElementById("tAssign");
  const tCategory = document.getElementById("tCategory");
  const tPriority = document.getElementById("tPriority");
  const tDate = document.getElementById("tDate");
  const tStatus = document.getElementById("tStatus");

  const openForm = () => formOverlay?.classList.add("open");
  const closeForm = () => formOverlay?.classList.remove("open");

  const resetCreateMode = () => {
    if (!form) return;
    form.reset();
    form.action = "/Admin/CreateTask";
    if (modalTitle) modalTitle.textContent = "Create Task";
  };

  openBtn?.addEventListener("click", () => {
    resetCreateMode();
    openForm();
  });

  cancelBtn?.addEventListener("click", closeForm);
  formOverlay?.addEventListener("click", (e) => {
    if (e.target === formOverlay) closeForm();
  });

  detailOverlay?.addEventListener("click", (e) => {
    if (e.target === detailOverlay) detailOverlay.classList.remove("open");
  });
  closeDetail?.addEventListener("click", () => detailOverlay?.classList.remove("open"));

  document.querySelectorAll(".edit-task").forEach((btn) => {
    btn.addEventListener("click", async () => {
      const id = btn.getAttribute("data-id");
      if (!id || !form) return;

      const response = await fetch(`/Admin/GetTask?id=${id}`);
      if (!response.ok) {
        alert("Could not load task data.");
        return;
      }

      const task = await response.json();
      form.action = `/Admin/EditTask/${id}`;
      if (modalTitle) modalTitle.textContent = "Edit Task";

      if (tTitle) tTitle.value = task.title ?? "";
      if (tDesc) tDesc.value = task.description ?? "";
      if (tAssign) tAssign.value = task.assignedToUserId ?? "";
      if (tCategory) tCategory.value = task.categoryId ?? "";
      if (tPriority) tPriority.value = task.priority ?? "";
      if (tStatus) tStatus.value = task.status ?? "";
      if (tDate) tDate.value = task.dueDate ? task.dueDate.substring(0, 10) : "";

      openForm();
    });
  });

  document.querySelectorAll(".view-task").forEach((btn) => {
    btn.addEventListener("click", () => {
      if (!detailBox || !detailOverlay) return;

      const rows = [
        ["Title", btn.getAttribute("data-title") || "-"],
        ["Description", btn.getAttribute("data-description") || "-"],
        ["Assigned To", btn.getAttribute("data-assigned") || "Unassigned"],
        ["Category", btn.getAttribute("data-category") || "-"],
        ["Priority", btn.getAttribute("data-priority") || "-"],
        ["Status", btn.getAttribute("data-status") || "-"],
        ["Due Date", btn.getAttribute("data-due") || "-"]
      ];

      detailBox.innerHTML = rows
        .map(([k, v]) => `<div><strong>${k}:</strong> ${v}</div>`)
        .join("");

      detailOverlay.classList.add("open");
    });
  });
});
