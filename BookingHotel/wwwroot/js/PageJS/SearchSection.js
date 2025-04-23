
const searchBar = document.getElementById("searchBar");
const originalOffsetTop = searchBar.offsetTop;

    window.addEventListener("scroll", () => {
        if (window.scrollY > originalOffsetTop + 50) {
    searchBar.classList.add("sticky-blur");
searchBar.classList.remove("normal");
        } else {
    searchBar.classList.remove("sticky-blur");
searchBar.classList.add("normal");
        }
    });
