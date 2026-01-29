window.addEventListener("scroll", function () {
    const nav = document.querySelector(".navbar");
    if (window.scrollY > 80) {
        nav.classList.add("scrolled");
    } else {
        nav.classList.remove("scrolled");
    }
});
