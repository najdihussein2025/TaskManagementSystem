document.addEventListener("DOMContentLoaded", () => {
  const overlay = document.getElementById("userModalOverlay");
  const openBtn = document.getElementById("openAddUser");
  const cancelBtn = document.getElementById("cancelUserModal");
  const form = document.getElementById("userForm");
  const title = document.getElementById("userModalTitle");
  const passwordField = document.querySelector(".password-only");

  const openModal = () => {
    title.textContent = "Add New User";
    if (passwordField) passwordField.style.display = "grid";
    overlay.classList.add("open");
  };

  const closeModal = () => {
    overlay.classList.remove("open");
    form?.reset();
  };

  openBtn?.addEventListener("click", openModal);
  cancelBtn?.addEventListener("click", closeModal);
  overlay?.addEventListener("click", (e) => {
    if (e.target === overlay) closeModal();
  });

  document.querySelectorAll(".edit-user, .delete-user").forEach((btn) => {
    btn.addEventListener("click", () => {
      // Keep UI responsive; backend actions should be wired server-side.
      overlay?.classList.remove("open");
    });
  });
});
