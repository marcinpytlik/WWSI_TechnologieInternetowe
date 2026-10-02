document.querySelector("#load").addEventListener("click", async () => {
  const result = document.querySelector("#result");

  try {
    const response = await fetch("/api/topics");
    const body = await response.json();

    result.textContent = JSON.stringify({
      status: response.status,
      data: body
    }, null, 2);
  } catch (error) {
    result.textContent = error.message;
  }
});
