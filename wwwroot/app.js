const authSection = document.getElementById("auth");
const appSection = document.getElementById("app");
const messageBox = document.getElementById("message");
const loginForm = document.getElementById("loginForm");
const registerForm = document.getElementById("registerForm");
const transactionForm = document.getElementById("transactionForm");
const categorySelect = document.getElementById("categorySelect");
const categoriesContainer = document.getElementById("categories");
const welcome = document.getElementById("welcome");
const userEmail = document.getElementById("userEmail");
const refreshBtn = document.getElementById("refreshBtn");
const logoutBtn = document.getElementById("logoutBtn");

const tokenKey = "expensive_token";
const userIdKey = "expensive_user_id";
const nameKey = "expensive_user_name";
const emailKey = "expensive_user_email";

const apiBase = "";

function getToken() {
  return localStorage.getItem(tokenKey);
}

function getUserId() {
  const value = localStorage.getItem(userIdKey);
  return value ? Number(value) : null;
}

function setAuth(data) {
  localStorage.setItem(tokenKey, data.token);
  localStorage.setItem(userIdKey, data.userId);
  localStorage.setItem(nameKey, data.name);
  localStorage.setItem(emailKey, data.email);
  updateUI();
}

function clearAuth() {
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(userIdKey);
  localStorage.removeItem(nameKey);
  localStorage.removeItem(emailKey);
  updateUI();
}

function authHeaders() {
  const token = getToken();
  return token
    ? {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      }
    : { "Content-Type": "application/json" };
}

function showMessage(text, isError = false) {
  messageBox.textContent = text;
  messageBox.classList.remove("hidden");
  messageBox.style.color = isError ? "#b00020" : "#111111";
  window.setTimeout(() => messageBox.classList.add("hidden"), 4000);
}

async function handleAuthSubmit(url, payload) {
  const response = await fetch(`${apiBase}${url}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    const errorText = response.status === 409 ? "Пользователь уже существует." : "Ошибка входа.";
    showMessage(errorText, true);
    return;
  }

  const data = await response.json();
  setAuth(data);
  await loadUser();
  showMessage("Готово");
}

loginForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  const formData = new FormData(loginForm);
  await handleAuthSubmit("/api/auth/login", {
    email: formData.get("email"),
    password: formData.get("password")
  });
});

registerForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  const formData = new FormData(registerForm);
  await handleAuthSubmit("/api/auth/register", {
    name: formData.get("name"),
    email: formData.get("email"),
    password: formData.get("password")
  });
});

transactionForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  const userId = getUserId();
  if (!userId) {
    showMessage("Нужно войти.", true);
    return;
  }

  const formData = new FormData(transactionForm);
  const amount = Number(formData.get("amount"));
  if (Number.isNaN(amount) || amount <= 0) {
    showMessage("Сумма должна быть больше 0.", true);
    return;
  }

  const categoryId = Number(categorySelect.value);
  const dateValue = formData.get("date");
  const date = dateValue ? new Date(`${dateValue}T00:00:00`).toISOString() : new Date().toISOString();

  const response = await fetch(
    `${apiBase}/api/users/${userId}/categories/${categoryId}/transactions`,
    {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify({
        amount,
        description: formData.get("description"),
        date
      })
    }
  );

  if (!response.ok) {
    showMessage("Не удалось добавить.", true);
    return;
  }

  transactionForm.reset();
  await loadUser();
  showMessage("Добавлено");
});

refreshBtn.addEventListener("click", async () => {
  await loadUser();
});

logoutBtn.addEventListener("click", () => {
  clearAuth();
});

async function loadUser() {
  const userId = getUserId();
  if (!userId) {
    return;
  }

  const response = await fetch(`${apiBase}/api/users/${userId}`, {
    headers: authHeaders()
  });

  if (response.status === 401 || response.status === 403) {
    clearAuth();
    showMessage("Сессия истекла.", true);
    return;
  }

  if (!response.ok) {
    showMessage("Не удалось загрузить данные.", true);
    return;
  }

  const user = await response.json();
  renderUser(user);
}

function renderUser(user) {
  welcome.textContent = `Имя: ${user.name}`;
  userEmail.textContent = user.email;
  categoriesContainer.innerHTML = "";
  categorySelect.innerHTML = "";

  user.categories.forEach((category) => {
    const option = document.createElement("option");
    option.value = category.id;
    option.textContent = `${category.name} (${category.type})`;
    categorySelect.appendChild(option);

    const wrapper = document.createElement("div");
    wrapper.className = "category";

    const title = document.createElement("h4");
    title.textContent = category.name;
    wrapper.appendChild(title);

    if (!category.transactions.length) {
      const empty = document.createElement("p");
      empty.className = "muted";
      empty.textContent = "Нет транзакций.";
      wrapper.appendChild(empty);
    } else {
      category.transactions.forEach((trx) => {
        const row = document.createElement("div");
        row.className = "transaction";
        row.innerHTML = `<span>${trx.description || "Без описания"}</span><span>${trx.amount}</span>`;
        wrapper.appendChild(row);
      });
    }

    categoriesContainer.appendChild(wrapper);
  });
}

function updateUI() {
  const token = getToken();
  const name = localStorage.getItem(nameKey);
  const email = localStorage.getItem(emailKey);

  if (token) {
    authSection.classList.add("hidden");
    appSection.classList.remove("hidden");
    welcome.textContent = name ? `Имя: ${name}` : "Пользователь";
    userEmail.textContent = email || "";
  } else {
    authSection.classList.remove("hidden");
    appSection.classList.add("hidden");
  }
}

updateUI();
if (getToken()) {
  loadUser();
}
