document.addEventListener('DOMContentLoaded', () => {
    const searchBar = document.getElementById("searchBar");
    if (!searchBar) {
        console.error("Element with id 'searchBar' not found!");
        return;
    }

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
});