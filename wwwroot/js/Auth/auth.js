document.addEventListener("DOMContentLoaded", () => {
  setupPasswordToggles();
  setupVisualPasswordStrength();
});

function setupPasswordToggles() {
  const toggles = document.querySelectorAll("[data-toggle-password]");
  toggles.forEach((toggle) => {
    toggle.addEventListener("click", () => {
      const targetId = toggle.getAttribute("data-toggle-password");
      const input = document.getElementById(targetId);
      if (!input) return;

      const showing = input.type === "text";
      input.type = showing ? "password" : "text";
      toggle.classList.toggle("is-open", !showing);
      toggle.setAttribute("aria-label", showing ? "Show password" : "Hide password");
    });
  });
}

function setupVisualPasswordStrength() {
  const passwordInput = document.getElementById("registerPassword");
  const strengthBar = document.getElementById("passwordStrengthBar");
  const strengthText = document.getElementById("passwordStrengthText");
  if (!passwordInput || !strengthBar || !strengthText) return;

  passwordInput.addEventListener("input", () => {
    updatePasswordStrength(passwordInput.value, strengthBar, strengthText);
  });
}

function updatePasswordStrength(password, bar, text) {
  if (!bar || !text) return;

  if (password.length < 6) {
    bar.style.width = "30%";
    bar.style.backgroundColor = "#ef4444";
    text.textContent = "Weak";
    return;
  }

  if (password.length <= 10) {
    bar.style.width = "65%";
    bar.style.backgroundColor = "#f59e0b";
    text.textContent = "Medium";
    return;
  }

  bar.style.width = "100%";
  bar.style.backgroundColor = "#22c55e";
  text.textContent = "Strong";
}
