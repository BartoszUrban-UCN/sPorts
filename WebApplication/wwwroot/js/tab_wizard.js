﻿var currentTab = 0; // Current tab is set to be the first tab (0)
showTab(currentTab); // Display the current tab

var boatId = null;
var startDate = null;
var endDate = null;

function clearAllCurrentData() {
    boatId = null;
    startDate = null;
    endDate = null;
}

function outputAllData() {
    console.log(`Boat ID is: ${boatId}\nStart Date is: ${startDate}\nEnd Date is: ${endDate}`);
}

// This function will display the specified tab of the form...
function showTab(n) {
    var x = document.getElementsByClassName("tab-wizard");
    x[n].style.display = "block";
    //... and fix the Previous/Next buttons:
    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }
    if (n == (x.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Submit";
    } else {
        document.getElementById("nextBtn").innerHTML = "Next";
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
            $(this).html("Your cart");
        $(this).fadeIn("fast");
    });
}

function fireFunction(n) {
    if (currentTab == 0) {
        if (n == 1) {
            boatId = $("#boat-select").val();
        }
    }
    else if (currentTab == 1) {
        if (n == -1) {
            startDate = null;
            endDate = null;
        }
        else if (n == 1) {
            startDate = $("#start-date").val();
            endDate = $("#end-date").val();
        }
    }
    else if (currentTab == 2) {
        if (n == -1) {
            
        }
        else if (n == 1) {
            
        }
    }
    else if (currentTab == 3) {
        if (n == -1) {

        }
        else if (n == 1) {

        }
    }
    else if (currentTab == 4) {
        if (n == -1) {

        }
        else if (n == 1) {

        }
    }

    outputAllData();
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
    // Increase or decrease the current tab by 1:
    currentTab = currentTab + n;
    // if you have reached the end of the form...
    if (currentTab >= x.length) {
        // ... the form gets submitted:
        document.getElementById("regForm").submit();
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