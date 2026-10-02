const API = "http://localhost:8081";

let token = null;

const loginForm = document.querySelector("#login-form");
const authStatus = document.querySelector("#auth-status");
const logoutButton = document.querySelector("#logout");
const profileButton = document.querySelector("#profile");
const profileResult = document.querySelector("#profile-result");
const loadTopicsButton = document.querySelector("#load-topics");
const topicsList = document.querySelector("#topics");

loginForm.addEventListener("submit", login);
logoutButton.addEventListener("click", logout);
profileButton.addEventListener("click", loadProfile);
loadTopicsButton.addEventListener("click", loadTopics);

async function login(event) {
  event.preventDefault();

  const username = document.querySelector("#username").value.trim();
  const password = document.querySelector("#password").value;

  const response = await fetch(`${API}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password })
  });

  if (!response.ok) {
    token = null;
    authStatus.textContent = `Logowanie nieudane: HTTP ${response.status}`;
    return;
  }

  const data = await response.json();

  token = data.token;
  authStatus.textContent = `Zalogowano: ${data.username}, rola: ${data.role}`;
  logoutButton.hidden = false;
}

function logout() {
  token = null;
  authStatus.textContent = "Niezalogowany";
  logoutButton.hidden = true;
  profileResult.textContent = "";
}

async function loadProfile() {
  const response = await fetch(`${API}/api/profile`, {
    headers: authHeaders()
  });

  profileResult.textContent = response.ok
    ? JSON.stringify(await response.json(), null, 2)
    : `HTTP ${response.status}`;
}

async function loadTopics() {
  const response = await fetch(`${API}/api/topics`);

  if (!response.ok) {
    topicsList.innerHTML = `<li>HTTP ${response.status}</li>`;
    return;
  }

  const items = await response.json();
  topicsList.replaceChildren();

  for (const item of items) {
    const li = document.createElement("li");
    li.textContent = `#${item.id} — ${item.name}`;
    topicsList.appendChild(li);
  }
}

function authHeaders() {
  return token
    ? { Authorization: `Bearer ${token}` }
    : {};
}
