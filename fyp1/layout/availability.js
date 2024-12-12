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
    const monthYearText = document.getElementById(lblMonth2ClientID).textContent;
    const [month, year] = monthYearText.split(' ');

    // Create a date object using template literals for proper formatting
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
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Add 1 to month to get the correct month number
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

function showEditAvailabilityModal(day, timeSlot, availableIDs) {
    // Get the current date from the calendar header (assuming it's in format "Month YYYY")
    const monthYearText = document.getElementById(lblMonth2ClientID).textContent;
    const [month, year] = monthYearText.split(' ');

    // Create a date object from the selected day using template literals
    const date = new Date(`${month} ${day}, ${year}`);

    // Format the date as YYYY-MM-DD for the input field
    const formattedDate = formatDate(date);

    // Show the modal first
    const modalElement = document.getElementById('editAvailabilityModal');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();

    // Wait for modal to be shown, then set the date and other details
    modalElement.addEventListener('shown.bs.modal', function () {
        // Prefill the date field
        document.querySelector('input[id$="txtEditDate"]').value = formattedDate;

        // Split the time slot into from and to times
        const [fromTime, toTime] = timeSlot.split(' - ');

        // Prefill the from and to time inputs
        document.querySelector('input[id$="txtEditAvailableFrom"]').value = fromTime;
        document.querySelector('input[id$="txtEditAvailableTo"]').value = toTime;

        // Set the available IDs (you can store it as a hidden field or whatever you need)
        document.querySelector('input[id$="hdnSelectedAvailableIDs"]').value = availableIDs;
    }, { once: true }); // Use once: true to ensure the event listener is removed after execution
}
