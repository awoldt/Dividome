const searchInput = document.getElementById("serach_input");
const searchResultsDiv = document.getElementById("search_results");

const divResults = document.createElement("div");

async function Search() {
  try {
    const query = searchInput.value;
    const req = await fetch("/search?q=" + query, {
      method: "POST",
    });

    if (req.status !== 200) {
      alert("Error while searching");
    } else {
      //if there are recent search results still on screen, remove all
      if (divResults.hasChildNodes()) {
        while (divResults.firstChild) {
          divResults.removeChild(divResults.firstChild);
        }
      }

      const res = await req.json();

      //there are results
      if (res.data.length > 0) {
        res.data.forEach((x) => {
          const link = document.createElement("a");
          link.innerText = x.name + " (" + x.symbol + ")";
          link.setAttribute("href", `/${x.symbol}`);
          link.style.display = "block";
          divResults.appendChild(link);
        });

        searchResultsDiv.appendChild(divResults);
      } else {
        alert("No results");
      }
    }
  } catch (e) {
    alert(e);
  }
}

searchInput.addEventListener("keyup", async (e) => {
  if (e.key === "Enter") {
    await Search();
  }
  if (searchInput.value === "") {
    //if there are recent search results still on screen, remove all
    if (divResults.hasChildNodes()) {
      while (divResults.firstChild) {
        divResults.removeChild(divResults.firstChild);
      }
    }
  }
});
