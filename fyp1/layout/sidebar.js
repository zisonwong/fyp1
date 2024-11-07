const toggler = document.querySelector(".sidebarBtn");
const sidebarToggleButton = document.getElementById("sidebarToggleButton");

toggler.addEventListener("click", function () {
    const sidebar = document.querySelector("#sidebar");
    const moveContent = document.querySelector("#moveContent");
    const screenWidth = window.innerWidth;

    if (screenWidth <= 768) {
        sidebar.classList.toggle("collapsed");
    } else {
        sidebar.classList.toggle("collapsed");
        moveContent.classList.toggle("collapsed");
    }
});

sidebarToggleButton.addEventListener("click", function () {
    const sidebar = document.querySelector("#sidebar");
    const screenWidth = window.innerWidth;

    if (screenWidth <= 768) {
        sidebar.classList.toggle("collapsed");
    }
});

window.addEventListener("DOMContentLoaded", function () {
    const screenWidth = window.innerWidth;
    const sidebar = document.querySelector("#sidebar");
    const moveContent = document.querySelector("#moveContent");

    if (screenWidth <= 768) {
        sidebar.classList.add("collapsed");
    }
});