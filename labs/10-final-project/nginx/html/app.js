let token = null;
let role = null;

const loginForm = document.querySelector("#login-form");
const topicForm = document.querySelector("#topic-form");
const topics = document.querySelector("#topics");
const status = document.querySelector("#status");
const authStatus = document.querySelector("#auth-status");

loginForm.addEventListener("submit", login);
topicForm.addEventListener("submit", saveTopic);

await loadTopics();

async function login(event) {
  event.preventDefault();

  const username = document.querySelector("#username").value.trim();
  const password = document.querySelector("#password").value;

  const response = await fetch("/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password })
  });

  if (!response.ok) {
    authStatus.textContent = `Logowanie nieudane: HTTP ${response.status}`;
    return;
  }

  const data = await response.json();
  token = data.token;
  role = data.role;
  authStatus.textContent = `Zalogowano: ${data.username} (${data.role})`;
  await loadTopics();
}

async function loadTopics() {
  status.textContent = "Ładowanie...";

  const response = await fetch("/api/topics");

  if (!response.ok) {
    status.textContent = `HTTP ${response.status}`;
    return;
  }

  const data = await response.json();

  topics.replaceChildren();

  for (const item of data) {
    const li = document.createElement("li");
    li.textContent = `#${item.id} — ${item.name} — ${item.description ?? ""}`;

    if (token) {
      const edit = document.createElement("button");
      edit.textContent = "Edytuj";
      edit.addEventListener("click", () => editTopic(item));
      li.appendChild(edit);
    }

    if (role === "Admin") {
      const remove = document.createElement("button");
      remove.textContent = "Usuń";
      remove.addEventListener("click", () => deleteTopic(item.id));
      li.appendChild(remove);
    }

    topics.appendChild(li);
  }

  status.textContent = `Rekordów: ${data.length}`;
}

async function saveTopic(event) {
  event.preventDefault();

  if (!token) {
    status.textContent = "Najpierw się zaloguj.";
    return;
  }

  const id = document.querySelector("#topic-id").value;
  const name = document.querySelector("#name").value.trim();
  const description = document.querySelector("#description").value.trim();

  const response = await fetch(id ? `/api/topics/${id}` : "/api/topics", {
    method: id ? "PUT" : "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify({ name, description })
  });

  if (!response.ok) {
    status.textContent = `HTTP ${response.status}`;
    return;
  }

  topicForm.reset();
  document.querySelector("#topic-id").value = "";
  await loadTopics();
}

function editTopic(item) {
  document.querySelector("#topic-id").value = item.id;
  document.querySelector("#name").value = item.name;
  document.querySelector("#description").value = item.description ?? "";
}

async function deleteTopic(id) {
  const response = await fetch(`/api/topics/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    status.textContent = `HTTP ${response.status}`;
    return;
  }

  await loadTopics();
}
