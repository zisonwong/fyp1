function updateClosingTimeOptions(openingTimeDropdown) {
    var selectedOpeningTime = openingTimeDropdown.value;
    var closingTimeDropdown = document.getElementById(openingTimeDropdown.id.replace("OpeningTime", "ClosingTime"));
    closingTimeDropdown.disabled = false;
    var previousClosingTime = closingTimeDropdown.value;
    closingTimeDropdown.innerHTML = "";
    var isValidPreviousSelection = false;
    var addOptions = false;
    for (var i = 0; i < openingTimeDropdown.options.length; i++) {
        var option = openingTimeDropdown.options[i];
        if (addOptions) {
            var newOption = document.createElement("option");
            newOption.value = option.value;
            newOption.text = option.text;
            closingTimeDropdown.add(newOption);
            if (option.value === previousClosingTime) {
                isValidPreviousSelection = true;
                closingTimeDropdown.value = previousClosingTime;
            }
        }
        if (option.value === selectedOpeningTime) {
            addOptions = true;
        }
    }
    if (!isValidPreviousSelection && closingTimeDropdown.options.length > 0) {
        closingTimeDropdown.selectedIndex = 0;
    }
}
var originalClosingOptions = [];
function filterClosingTimeOptions(openingTimeDDLId, closingTimeDDLId) {
    var openingTimeDropdown = document.getElementById(openingTimeDDLId);
    var closingTimeDropdown = document.getElementById(closingTimeDDLId);
    // Get the selected opening time value
    var selectedOpeningTimeIndex = openingTimeDropdown.selectedIndex;
    var selectedOpeningTime = openingTimeDropdown.options[selectedOpeningTimeIndex].value;
    // If original options are not stored, store them
    if (originalClosingOptions.length === 0) {
        for (var i = 0; i < closingTimeDropdown.options.length; i++) {
            originalClosingOptions.push({
                value: closingTimeDropdown.options[i].value,
                text: closingTimeDropdown.options[i].text
            });
        }
    }
    // Check if the selected opening time is the default ("Opening Time")
    if (selectedOpeningTime === "") {
        // Restore the original closing dropdown options
        closingTimeDropdown.innerHTML = "";
        originalClosingOptions.forEach(function (option) {
            var newOption = document.createElement("option");
            newOption.value = option.value;
            newOption.text = option.text;
            closingTimeDropdown.add(newOption);
        });
        closingTimeDropdown.selectedIndex = 0;
        return;
    }
    // Clear the closing dropdown and add only valid options after selected opening time
    closingTimeDropdown.innerHTML = "<option value=''>Closing Time</option>";  // Add default option at the top
    // Populate closing dropdown with options greater than the selected opening time
    for (var i = 0; i < openingTimeDropdown.options.length; i++) {
        if (i > selectedOpeningTimeIndex) {  // Only options after selected opening time
            var option = openingTimeDropdown.options[i];
            var newOption = document.createElement("option");
            newOption.value = option.value;
            newOption.text = option.text;
            closingTimeDropdown.add(newOption);
        }
    }
}



