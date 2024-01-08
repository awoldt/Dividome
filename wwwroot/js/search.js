const searchInput = document.getElementById("search_input");

async function Search() {
  try {
    const req = await fetch("/api/search", {
      method: "post",
      body: JSON.stringify({
        Ticker: searchInput.value,
      }),
      headers: {
        "content-type": "application/json",
      },
    });

    const res = await req.json();
    if (res.valid) {
      window.location.assign(`/${searchInput.value}`);
    }
  } catch (err) {
    alert(err);
  }
}

// Add an event listener to the input element
searchInput.addEventListener("keyup", async function (event) {
  if (event.key === "Enter") {
    await Search();
  }
});
