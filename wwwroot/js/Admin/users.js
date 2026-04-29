document.addEventListener("DOMContentLoaded", () => {
  const overlay = document.getElementById("userModalOverlay");
  const openBtn = document.getElementById("openAddUser");
  const cancelBtn = document.getElementById("cancelUserModal");
  const title = document.getElementById("userModalTitle");
  const passwordField = document.querySelector(".password-only");
  const form = document.getElementById("userForm");
  const nameInput = document.getElementById("userName");
  const emailInput = document.getElementById("userEmail");
  const roleInput = document.getElementById("userRole");
  const statusInput = document.getElementById("userStatus");
  let editRow = null;

  const openModal = (isEdit) => {
    title.textContent = isEdit ? "Edit User" : "Add New User";
    passwordField.style.display = isEdit ? "none" : "grid";
    overlay.classList.add("open");
  };
  const closeModal = () => { overlay.classList.remove("open"); form.reset(); editRow = null; };
  openBtn?.addEventListener("click", () => openModal(false));
  cancelBtn?.addEventListener("click", closeModal);
  overlay?.addEventListener("click", (e) => { if (e.target === overlay) closeModal(); });

  document.querySelectorAll(".edit-user").forEach((btn) => {
    btn.addEventListener("click", () => {
      editRow = btn.closest("tr");
      nameInput.value = editRow.dataset.name;
      emailInput.value = editRow.dataset.email;
      roleInput.value = editRow.dataset.role;
      statusInput.value = editRow.dataset.status;
      openModal(true);
    });
  });

  form?.addEventListener("submit", (e) => { e.preventDefault(); closeModal(); });

  document.querySelectorAll(".delete-user").forEach((btn) => {
    btn.addEventListener("click", () => btn.closest("tr").classList.add("deleting"));
  });
  document.querySelectorAll(".confirm-delete .no").forEach((btn) => btn.addEventListener("click", () => btn.closest("tr").classList.remove("deleting")));
  document.querySelectorAll(".confirm-delete .yes").forEach((btn) => btn.addEventListener("click", () => btn.closest("tr").remove()));

  const applyFilters = () => {
    const q = document.getElementById("userSearch").value.toLowerCase();
    const role = document.getElementById("roleFilter").value;
    const status = document.getElementById("statusFilter").value;
    document.querySelectorAll("#usersTable tbody tr").forEach((row) => {
      const hitQ = row.dataset.name.toLowerCase().includes(q) || row.dataset.email.toLowerCase().includes(q);
      const hitRole = role === "All Roles" || row.dataset.role === role;
      const hitStatus = status === "All Status" || row.dataset.status === status;
      row.style.display = hitQ && hitRole && hitStatus ? "" : "none";
    });
  };
  document.getElementById("applyFilters")?.addEventListener("click", applyFilters);
  document.getElementById("userSearch")?.addEventListener("input", applyFilters);
});
