
var mymap;
mymap = initMap('map-marina');
var marinaMarkers = L.layerGroup().addTo(mymap);
mymap.setView([56.2639, 9.5018], 7);

function renderMarinas(marinas) {
    marinaMarkers.clearLayers();
    marinas.forEach(marina => {
        if (marina.Location) {
            marinaBookingPrompt(marina, 5)
        }
    })
}

/**
 * 
 * @param {marina} marina - marina model
 * @param {number} numberOfAvailableSpots - number of available spots in the marina
 * @returns {L.Circle} - use this to bind further commands like binding a popup 
 */
function marinaBookingPrompt(marina, numberOfAvailableSpots) {
    var marinaPopup = `\
        <div class="text-center" style="width: 130px">\
            <p style="font-weight: bold; color: #737373">${marina.Name}</p>\
            <p style="font-weight: bold; color: #376186">${numberOfAvailableSpots} spots available!</p>\
            <button type="button" id="bookButton" class="btn btn-primary btn-sm" onclick="selectMarina(${marina.MarinaId})">Book</button>\
        </div>\
    `;

    renderCircle(marina.Location, marinaMarkers).bindPopup(marinaPopup);
}

/**
 * The function that the marinaBookingPrompt button calls onClick
 * @param {number} marinaId - the id of the marina
 */
function selectMarina(marinaId) {
    document.getElementById("selectedMarina").value = marinaId;
    document.getElementById("nextBtn").click();
}