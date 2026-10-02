const form = document.querySelector("#greeting-form");
const nameInput = document.querySelector("#name");
const greeting = document.querySelector("#greeting");

const loadCourseButton = document.querySelector("#load-course");
const courseStatus = document.querySelector("#course-status");
const topicsList = document.querySelector("#topics");

form.addEventListener("submit", event => {
  event.preventDefault();

  const name = nameInput.value.trim();

  if (!name) {
    greeting.textContent = "Podaj imię.";
    greeting.classList.add("error");
    return;
  }

  greeting.classList.remove("error");
  greeting.textContent = `Cześć ${name}! JavaScript właśnie zmienił DOM.`;
});

loadCourseButton.addEventListener("click", loadCourse);

async function loadCourse() {
  courseStatus.classList.remove("error");
  courseStatus.textContent = "Ładowanie...";
  topicsList.replaceChildren();

  try {
    const response = await fetch("/api/course.json");

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const data = await response.json();

    courseStatus.textContent =
      `${data.name} — ${data.hours}h, bloków: ${data.blocks}`;

    for (const topic of data.topics) {
      const li = document.createElement("li");
      li.textContent = topic;
      topicsList.appendChild(li);
    }
  } catch (error) {
    courseStatus.textContent = `Nie udało się pobrać danych: ${error.message}`;
    courseStatus.classList.add("error");
  }
}
