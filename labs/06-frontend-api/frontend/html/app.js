const API_BASE = "http://localhost:8081";

const form = document.querySelector("#topic-form");
const idInput = document.querySelector("#topic-id");
const nameInput = document.querySelector("#topic-name");
const submitButton = document.querySelector("#submit-button");
const cancelButton = document.querySelector("#cancel-button");
const refreshButton = document.querySelector("#refresh-button");
const statusElement = document.querySelector("#status");
const list = document.querySelector("#topics-list");

refreshButton.addEventListener("click", loadTopics);
cancelButton.addEventListener("click", resetForm);
form.addEventListener("submit", saveTopic);

await loadTopics();

async function loadTopics() {
  setStatus("Ładowanie...");
  list.replaceChildren();

  try {
    const response = await fetch(`${API_BASE}/api/topics?page=1&pageSize=100`);

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const data = await response.json();

    for (const topic of data.items) {
      list.appendChild(createTopicElement(topic));
    }

    setStatus(`Liczba tematów: ${data.totalItems}`);
  } catch (error) {
    setStatus(`Nie udało się pobrać danych: ${error.message}`, true);
  }
}

async function saveTopic(event) {
  event.preventDefault();

  const id = idInput.value;
  const name = nameInput.value.trim();

  if (!name) return;

  const isEdit = Boolean(id);
  const url = isEdit
    ? `${API_BASE}/api/topics/${id}`
    : `${API_BASE}/api/topics`;

  setBusy(true);
  setStatus(isEdit ? "Zapisywanie zmian..." : "Dodawanie...");

  try {
    const response = await fetch(url, {
      method: isEdit ? "PUT" : "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ name })
    });

    if (!response.ok) {
      const problem = await safeJson(response);
      throw new Error(problem?.detail ?? `HTTP ${response.status}`);
    }

    resetForm();
    await loadTopics();
  } catch (error) {
    setStatus(`Błąd zapisu: ${error.message}`, true);
  } finally {
    setBusy(false);
  }
}

async function deleteTopic(id) {
  if (!confirm("Usunąć temat?")) return;

  setStatus("Usuwanie...");

  try {
    const response = await fetch(`${API_BASE}/api/topics/${id}`, {
      method: "DELETE"
    });

    if (!response.ok) {
      const problem = await safeJson(response);
      throw new Error(problem?.detail ?? `HTTP ${response.status}`);
    }

    await loadTopics();
  } catch (error) {
    setStatus(`Błąd usuwania: ${error.message}`, true);
  }
}

function editTopic(topic) {
  idInput.value = topic.id;
  nameInput.value = topic.name;
  submitButton.textContent = "Zapisz";
  cancelButton.hidden = false;
  nameInput.focus();
}

function resetForm() {
  form.reset();
  idInput.value = "";
  submitButton.textContent = "Dodaj";
  cancelButton.hidden = true;
}

function createTopicElement(topic) {
  const li = document.createElement("li");
  li.className = "topic-item";

  const name = document.createElement("span");
  name.textContent = `#${topic.id} — ${topic.name}`;

  const actions = document.createElement("div");
  actions.className = "topic-actions";

  const edit = document.createElement("button");
  edit.type = "button";
  edit.textContent = "Edytuj";
  edit.addEventListener("click", () => editTopic(topic));

  const remove = document.createElement("button");
  remove.type = "button";
  remove.textContent = "Usuń";
  remove.addEventListener("click", () => deleteTopic(topic.id));

  actions.append(edit, remove);
  li.append(name, actions);

  return li;
}

function setBusy(busy) {
  submitButton.disabled = busy;
  refreshButton.disabled = busy;
}

function setStatus(message, isError = false) {
  statusElement.textContent = message;
  statusElement.classList.toggle("error", isError);
}

async function safeJson(response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}
