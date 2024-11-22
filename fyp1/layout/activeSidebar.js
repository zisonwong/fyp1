// Function to add active class to the current page's link
window.addEventListener('DOMContentLoaded', (event) => {
    // Get the current page's URL without query parameters
    var currentPageUrl = window.location.pathname; // This gets the path without query strings

    // Get all navigation links
    var navLinks = document.querySelectorAll('#sidebar .nav-link');

    // Loop through each link
    navLinks.forEach(function (link) {
        // Compare the link's pathname without query parameters
        if (link.pathname === currentPageUrl) {
            // Add the active class to the link
            link.classList.add('activate');
            link.classList.add('changeColor');
        }
    });
});