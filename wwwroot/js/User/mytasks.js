document.addEventListener("DOMContentLoaded", () => {
  const overlay = document.getElementById("taskDetailOverlay");
  const closeBtn = document.getElementById("closeTaskDetail");
  const detailButtons = document.querySelectorAll(".details-link");
  const commentsList = document.getElementById("commentsList");
  const commentInput = document.getElementById("commentInput");
  const addCommentBtn = document.getElementById("addCommentBtn");
  const clearCommentBtn = document.getElementById("clearCommentBtn");

  detailButtons.forEach((button) => {
    button.addEventListener("click", () => {
      overlay?.classList.add("open");
    });
  });

  closeBtn?.addEventListener("click", () => {
    overlay?.classList.remove("open");
  });

  overlay?.addEventListener("click", (event) => {
    if (event.target === overlay) {
      overlay.classList.remove("open");
    }
  });

  addCommentBtn?.addEventListener("click", () => {
    const message = commentInput?.value.trim();
    if (!message || !commentsList) return;

    const item = document.createElement("article");
    item.className = "comment-item";
    item.innerHTML = `
      <div class="comment-avatar">JD</div>
      <div class="comment-body">
        <div class="comment-head">
          <strong>John Doe</strong>
          <small>${formatCommentTime(new Date())}</small>
        </div>
        <p>${escapeHtml(message)}</p>
      </div>
    `;

    commentsList.appendChild(item);
    commentsList.scrollTop = commentsList.scrollHeight;
    commentInput.value = "";
  });

  clearCommentBtn?.addEventListener("click", () => {
    if (commentInput) {
      commentInput.value = "";
    }
  });
});

function formatCommentTime(date) {
  return date.toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "numeric",
    minute: "2-digit"
  });
}

function escapeHtml(text) {
  const map = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    "\"": "&quot;",
    "'": "&#39;"
  };

  return text.replace(/[&<>"']/g, (char) => map[char]);
}
