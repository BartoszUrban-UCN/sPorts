var currentTab = 0; // Current tab is set to be the first tab (0)
showTab(currentTab); // Display the current tab

var boatId = null;
var startDate = null;
var endDate = null;
var marinaId = null;
var spotId = null;
var booking = null;

function outputAllData() {
    console.log(`Boat ID is: ${boatId}\nStart Date is: ${startDate}\nEnd Date is: ${endDate}\nSelected Marina is: ${marinaId}\nSelected Spot is: ${spotId}`);
    console.log(booking);
}

// This function will display the specified tab of the form...
function showTab(n) {
    var x = document.getElementsByClassName("tab-wizard");
    x[n].style.display = "block";
    //... and fix the Previous/Next buttons:
    if (n == 0 || n == 4) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }

    if (n == 2 || n == 3) {
        document.getElementById("nextBtn").style.display = "none";
    } else if (n == (x.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Choose more dates";
        document.getElementById("nextBtn").style.display = "inline";
    } else {
        document.getElementById("nextBtn").innerHTML = "Next";
        document.getElementById("nextBtn").style.display = "inline";
    }

    //... and run a function that will display the correct step indicator:
    updateStepIndicator(n);
    updateTitle();
}

function updateTitle() {
    $("#dynamic-title").fadeOut("fast", function () {
        if (currentTab == 0)
            $(this).html("Choose your boat");
        else if (currentTab == 1)
            $(this).html("Choose your booking's dates");
        else if (currentTab == 2)
            $(this).html("Which marina?");
        else if (currentTab == 3)
            $(this).html("Now... Which spot?");
        else if (currentTab == 4)
            $(this).html("Spot successfuly added to cart!");
        $(this).fadeIn("fast");
    });
}

// This function will figure out which tab to display
function nextPrev(n) {
    var x = document.getElementsByClassName("tab-wizard");
    // Don't let people go next if the form is invalid
    if (n == 1 && !validateForm()) return false;
    // Fire functions
    fireFunction(n);
    // Hide the current tab:
    x[currentTab].style.display = "none";

    currentTab = currentTab + n;
    // If you have reached the end of the form and want to add more dates
    if (currentTab >= x.length) {
        currentTab = 1;
        fireFunction(currentTab);
        showTab(currentTab);
        return false;
    }
    // Otherwise, display the correct tab:
    showTab(currentTab);
}

function validateForm() {
    // This function deals with validation of the form fields
    var valid = true;

    // Get information about the booking form
    valid = $("#booking").valid();

    // If the valid status is true, mark the step as finished and valid:
    if (valid)
        document.getElementsByClassName("step")[currentTab].className += " finish";

    return valid; // return the valid status
}

function updateStepIndicator(n) {
    // This function removes the "active" class of all steps...
    var i, x = document.getElementsByClassName("step");
    for (i = 0; i < x.length; i++) {
        x[i].className = x[i].className.replace(" active", "");
    }
    //... and adds the "active" class on the current step:
    x[n].className += " active";
}