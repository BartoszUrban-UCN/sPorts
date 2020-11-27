var mymapspot;
mymapspot = initMap('map-spot');
var spotMarkers = L.layerGroup().addTo(mymapspot);
mymapspot.setView([56.2639, 9.5018], 7);

function renderSpots(spots) {
    spotMarkers.clearLayers();
    spots.forEach(spot => {
        if (spot.Location) {
            spotBookingPrompt(spot, true)
            mymapspot.flyTo([parseFloat(spot.Location.Latitude), parseFloat(spot.Location.Longitude)], 16);
        }
    })
}

/**
 * 
 * @param {spot} spot - spot model
 * @param {boolean} available - availability of the spot
 * @returns {L.Marker} - use this to bind further commands like binding a popup
 */
function spotBookingPrompt(spot, available) {
    var spotPopup = `\
        <div class="text-center" style="width: 130px">\
            <p style="font-weight: bold; color: #737373">Spot number ${spot.SpotNumber}</p>\
            <p style="font-weight: bold; color: #376186">Price per day: ${spot.Price}</p>\
            <button type="button" id="bookButton" class="btn btn-primary btn-sm" onclick="selectSpot(${spot.SpotId})">Book</button>\
        </div>\
    `;

    renderMarkerLocation(spot.Location, spotMarkers, available).bindPopup(spotPopup);
}
