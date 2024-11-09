// Add event listeners when the document is ready
document.addEventListener('DOMContentLoaded', function () {
    // Add click event listeners to all clickable dates
    document.querySelectorAll('.clickable-date').forEach(function (element) {
        element.addEventListener('click', function () {
            const day = this.dataset.date;
            showAvailabilityModal(day);
        });
    });
});

function showAvailabilityModal(day) {
    // Get the current date from the calendar header (assuming it's in format "Month YYYY")
    const monthYearText = document.getElementById('lblMonth').textContent;
    const [month, year] = monthYearText.split(' ');

    // Create a date object
    const date = new Date(`${month} ${day}, ${year}`);

    // Format the date as YYYY-MM-DD for the input field
    const formattedDate = formatDate(date);

    // Show the modal first
    const modalElement = document.getElementById('availabilityModal');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();

    // Wait for modal to be shown, then set the date
    modalElement.addEventListener('shown.bs.modal', function () {
        document.querySelector('input[id$="txtDate"]').value = formattedDate;
    }, { once: true }); // Use once: true to ensure the event listener is removed after execution
}

function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}


