document.addEventListener("DOMContentLoaded", () => {
  const overlay = document.getElementById("userModalOverlay");
  const openBtn = document.getElementById("openAddUser");
  const cancelBtn = document.getElementById("cancelUserModal");
  const form = document.getElementById("userForm");
  const title = document.getElementById("userModalTitle");
  const passwordField = document.querySelector(".password-only");
  const idInput = document.getElementById("userId");
  const nameInput = document.getElementById("userName");
  const emailInput = document.getElementById("userEmail");
  const roleInput = document.getElementById("userRole");
  const statusInput = document.getElementById("userStatus");
  const filterForm = document.getElementById("usersFilterForm");
  const searchInput = document.getElementById("userSearch");
  const listStatusFilter = document.getElementById("statusFilter");

  const openCreateModal = () => {
    title.textContent = "Add New User";
    if (passwordField) passwordField.style.display = "grid";
    if (form) form.action = "/Admin/CreateUser";
    if (idInput) idInput.value = "";
    overlay.classList.add("open");
  };

  const openEditModal = (row) => {
    title.textContent = "Edit User";
    if (passwordField) passwordField.style.display = "none";
    if (form) form.action = "/Admin/UpdateUser";

    if (idInput) idInput.value = row.dataset.id || "";
    if (nameInput) nameInput.value = row.dataset.name || "";
    if (emailInput) emailInput.value = row.dataset.email || "";
    if (roleInput) roleInput.value = row.dataset.role || "User";
    if (statusInput) statusInput.value = row.dataset.status || "Active";

    overlay.classList.add("open");
  };

  const closeModal = () => {
    overlay.classList.remove("open");
    form?.reset();
  };

  openBtn?.addEventListener("click", openCreateModal);
  cancelBtn?.addEventListener("click", closeModal);
  overlay?.addEventListener("click", (e) => {
    if (e.target === overlay) closeModal();
  });

  document.querySelectorAll(".edit-user").forEach((btn) => {
    btn.addEventListener("click", () => {
      const row = btn.closest("tr");
      if (!row) return;
      openEditModal(row);
    });
  });

  const debounce = (fn, delay = 250) => {
    let timeoutId;
    return (...args) => {
      clearTimeout(timeoutId);
      timeoutId = setTimeout(() => fn(...args), delay);
    };
  };

  const submitFilters = () => {
    filterForm?.requestSubmit();
  };

  searchInput?.addEventListener("input", debounce(submitFilters, 250));
  listStatusFilter?.addEventListener("change", submitFilters);

});
