
var mymap;
mymap = initMap('map-marina');
var marinaMarkers = L.layerGroup().addTo(mymap);
mymap.setView([56.2639, 9.5018], 7);

function renderMarinas(marinas) {
    marinaMarkers.clearLayers();
    marinas.forEach(marina => {
        if (marina.Key.Location) {
            marinaBookingPrompt(marina.Key, marina.Value)
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

    renderCircleLocation(marina.Location, marinaMarkers).bindPopup(marinaPopup);
}