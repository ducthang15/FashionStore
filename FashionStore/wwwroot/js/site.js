window.addEventListener("scroll", function () {
    const nav = document.querySelector(".navbar");
    if (window.scrollY > 80) {
        nav.classList.add("scrolled");
    } else {
        nav.classList.remove("scrolled");
    }
});
const btn = document.getElementById("backToTop");

// Ẩn lúc đầu
btn.style.display = "none";

document.addEventListener("scroll", () => {
    const scroll = document.documentElement.scrollTop;

    if (scroll > 300) {
        btn.style.display = "block";
    } else {
        btn.style.display = "none";
    }
});

btn.addEventListener("click", (e) => {
    e.preventDefault();

    document.documentElement.scrollTo({
        top: 0,
        behavior: "smooth"
    });
});