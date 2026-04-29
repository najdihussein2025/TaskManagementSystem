document.addEventListener("DOMContentLoaded", () => {
  setupPasswordToggles();
  setupLoginForm();
  setupRegisterForm();
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

function setupLoginForm() {
  const form = document.getElementById("loginForm");
  if (!form) return;

  const emailInput = document.getElementById("loginEmail");
  const passwordInput = document.getElementById("loginPassword");

  emailInput.addEventListener("blur", () => validateEmail(emailInput));
  passwordInput.addEventListener("blur", () => validatePassword(passwordInput, 6));
  emailInput.addEventListener("input", () => clearSuccess(form));
  passwordInput.addEventListener("input", () => clearSuccess(form));

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    clearSuccess(form);

    const emailOk = validateEmail(emailInput);
    const passwordOk = validatePassword(passwordInput, 6);
    if (!emailOk || !passwordOk) return;

    await fakeSubmit(form, "Login successful! Redirecting...");
  });
}

function setupRegisterForm() {
  const form = document.getElementById("registerForm");
  if (!form) return;

  const nameInput = document.getElementById("registerName");
  const emailInput = document.getElementById("registerEmail");
  const passwordInput = document.getElementById("registerPassword");
  const confirmInput = document.getElementById("registerConfirmPassword");
  const termsCheckbox = document.getElementById("termsCheckbox");
  const strengthBar = document.getElementById("passwordStrengthBar");
  const strengthText = document.getElementById("passwordStrengthText");

  nameInput.addEventListener("blur", () => validateRequired(nameInput, "Full Name is required."));
  emailInput.addEventListener("blur", () => validateEmail(emailInput));
  passwordInput.addEventListener("blur", () => validatePassword(passwordInput, 6));
  confirmInput.addEventListener("blur", () => validatePasswordMatch(passwordInput, confirmInput));
  termsCheckbox.addEventListener("change", () => validateTerms(termsCheckbox));

  passwordInput.addEventListener("input", () => {
    updatePasswordStrength(passwordInput.value, strengthBar, strengthText);
    clearSuccess(form);
  });
  confirmInput.addEventListener("input", () => clearSuccess(form));
  nameInput.addEventListener("input", () => clearSuccess(form));
  emailInput.addEventListener("input", () => clearSuccess(form));

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    clearSuccess(form);

    const nameOk = validateRequired(nameInput, "Full Name is required.");
    const emailOk = validateEmail(emailInput);
    const passOk = validatePassword(passwordInput, 6);
    const matchOk = validatePasswordMatch(passwordInput, confirmInput);
    const termsOk = validateTerms(termsCheckbox);

    if (!nameOk || !emailOk || !passOk || !matchOk || !termsOk) return;

    await fakeSubmit(form, "Account created! Please sign in.");
  });
}

function validateRequired(input, message) {
  const value = input.value.trim();
  if (!value) {
    setFieldError(input, message);
    return false;
  }
  setFieldValid(input);
  return true;
}

function validateEmail(input) {
  const value = input.value.trim();
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(value)) {
    setFieldError(input, "Please enter a valid email address.");
    return false;
  }
  setFieldValid(input);
  return true;
}

function validatePassword(input, minLength) {
  if (input.value.length < minLength) {
    setFieldError(input, `Password must be at least ${minLength} characters.`);
    return false;
  }
  setFieldValid(input);
  return true;
}

function validatePasswordMatch(passwordInput, confirmInput) {
  if (confirmInput.value !== passwordInput.value || !confirmInput.value) {
    setFieldError(confirmInput, "Passwords do not match.");
    return false;
  }
  setFieldValid(confirmInput);
  return true;
}

function validateTerms(checkbox) {
  if (!checkbox.checked) {
    setCheckboxError(checkbox, "You must agree before creating an account.");
    return false;
  }
  clearErrorById(checkbox.id);
  return true;
}

function setFieldError(input, message) {
  input.classList.add("input-error");
  input.classList.remove("input-valid");
  const errorEl = getErrorElement(input.id);
  if (!errorEl) return;
  errorEl.textContent = message;
  errorEl.classList.add("visible");
}

function setFieldValid(input) {
  input.classList.remove("input-error");
  input.classList.add("input-valid");
  clearErrorById(input.id);
}

function setCheckboxError(checkbox, message) {
  const errorEl = getErrorElement(checkbox.id);
  if (!errorEl) return;
  errorEl.textContent = message;
  errorEl.classList.add("visible");
}

function clearErrorById(inputId) {
  const errorEl = getErrorElement(inputId);
  if (!errorEl) return;
  errorEl.textContent = "";
  errorEl.classList.remove("visible");
}

function getErrorElement(inputId) {
  return document.querySelector(`[data-error-for="${inputId}"]`);
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

async function fakeSubmit(form, successText) {
  const button = form.querySelector(".submit-btn");
  const success = form.querySelector(`[data-success-for="${form.id}"]`);
  if (!button || !success) return;

  button.disabled = true;
  button.classList.add("loading");
  success.classList.remove("visible");

  await wait(1500);

  button.disabled = false;
  button.classList.remove("loading");
  success.textContent = successText;
  success.classList.add("visible");
}

function clearSuccess(form) {
  const success = form.querySelector(`[data-success-for="${form.id}"]`);
  if (!success) return;
  success.classList.remove("visible");
}

function wait(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
