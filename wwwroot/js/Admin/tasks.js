document.addEventListener("DOMContentLoaded", () => {
  const formOverlay = document.getElementById("taskFormOverlay");
  const detailOverlay = document.getElementById("taskDetailOverlay");
  const openBtn = document.getElementById("openTaskModal");
  const cancelBtn = document.getElementById("cancelTaskModal");
  const closeDetail = document.getElementById("closeTaskDetail");
  const form = document.getElementById("taskForm");
  const title = document.getElementById("taskModalTitle");
  const detailBox = document.getElementById("taskDetailBox");

  const fields = { tTitle: "task", tAssign: "user", tCategory: "category", tPriority: "priority", tStatus: "status" };
  let editingRow = null;

  const openForm = (isEdit) => {
    title.textContent = isEdit ? "Edit Task" : "Create Task";
    formOverlay.classList.add("open");
  };
  const closeForm = () => { formOverlay.classList.remove("open"); form.reset(); editingRow = null; };

  openBtn?.addEventListener("click", () => openForm(false));
  cancelBtn?.addEventListener("click", closeForm);
  formOverlay?.addEventListener("click", (e) => { if (e.target === formOverlay) closeForm(); });
  detailOverlay?.addEventListener("click", (e) => { if (e.target === detailOverlay) detailOverlay.classList.remove("open"); });
  closeDetail?.addEventListener("click", () => detailOverlay.classList.remove("open"));
  form?.addEventListener("submit", (e) => { e.preventDefault(); closeForm(); });

  document.querySelectorAll(".edit-task").forEach((btn) => btn.addEventListener("click", () => {
    editingRow = btn.closest("tr");
    document.getElementById("tTitle").value = editingRow.dataset.task;
    document.getElementById("tAssign").value = editingRow.dataset.user;
    document.getElementById("tCategory").value = editingRow.dataset.category;
    document.getElementById("tPriority").value = editingRow.dataset.priority;
    document.getElementById("tStatus").value = editingRow.dataset.status;
    openForm(true);
  }));

  document.querySelectorAll(".view-task").forEach((btn) => btn.addEventListener("click", () => {
    const row = btn.closest("tr");
    detailBox.innerHTML = `<strong>Title</strong><span>${row.dataset.task}</span><strong>Assigned To</strong><span>${row.dataset.user}</span><strong>Category</strong><span>${row.dataset.category}</span><strong>Priority</strong><span>${row.dataset.priority}</span><strong>Due Date</strong><span>${row.dataset.date}</span><strong>Status</strong><span>${row.dataset.status}</span>`;
    detailOverlay.classList.add("open");
  }));

  document.querySelectorAll(".delete-task").forEach((btn) => btn.addEventListener("click", () => btn.closest("tr").classList.add("deleting")));
  document.querySelectorAll(".confirm-delete .no").forEach((btn) => btn.addEventListener("click", () => btn.closest("tr").classList.remove("deleting")));
  document.querySelectorAll(".confirm-delete .yes").forEach((btn) => btn.addEventListener("click", () => btn.closest("tr").remove()));

  const applyFilters = () => {
    const query = document.getElementById("taskSearch").value.toLowerCase();
    const s = document.getElementById("statusFilter").value;
    const p = document.getElementById("priorityFilter").value;
    const c = document.getElementById("categoryFilter").value;
    const u = document.getElementById("userFilter").value;
    document.querySelectorAll("#tasksTable tbody tr").forEach((row) => {
      const okQ = row.dataset.task.toLowerCase().includes(query);
      const okS = s === "All Status" || row.dataset.status === s;
      const okP = p === "All Priority" || row.dataset.priority === p;
      const okC = c === "All Categories" || row.dataset.category === c;
      const okU = u === "All Users" || row.dataset.user === u;
      row.style.display = okQ && okS && okP && okC && okU ? "" : "none";
    });
  };
  document.getElementById("applyTaskFilters")?.addEventListener("click", applyFilters);
  document.getElementById("taskSearch")?.addEventListener("input", applyFilters);
});
