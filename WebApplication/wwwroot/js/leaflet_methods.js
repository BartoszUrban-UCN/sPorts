function MarinaBookingPrompt(marina, numberOfAvailableSpots) {
    var marinaPopup = `\
        <div class="text-center" style="width: 130px">\
            <p style="font-weight: bold; color: #737373">${marina.Name}</p>\
            <p style="font-weight: bold; color: #376186">${numberOfAvailableSpots} spots available!</p>\
            <button type="button" id="bookButton" class="btn btn-primary btn-sm" onclick="selectMarina(${marina.MarinaId})">Book</button>\
        </div>\
    `;

    L.circle([parseFloat(marina.Location.Latitude), parseFloat(marina.Location.Longitude)], {
        color: 'pink',
        fillColor: '#f03',
        fillOpacity: 0.5,
        radius: ((parseFloat(marina.Location.Radius) + 0.1) * 1000),
    }).bindPopup(marinaPopup).addTo(marinaMarkers);
}

function SpotBookingPrompt(spot, available) {
    var spotPopup = `\
        <div class="text-center" style="width: 130px">\
            <p style="font-weight: bold; color: #737373">Spot number ${spot.SpotNumber}</p>\
            <p style="font-weight: bold; color: #376186">Price per day: ${spot.Price}</p>\
            <button type="button" id="bookButton" class="btn btn-primary btn-sm">Book</button>\
        </div>\
    `;

    if (available) {
        L.marker([parseFloat(spot.Location.Latitude), parseFloat(spot.Location.Longitude)]).bindPopup(spotPopup).addTo(spotMarkers)
    }
    else {
        L.marker([parseFloat(spot.Location.Latitude), parseFloat(spot.Location.Longitude)], { icon: unavailableMarkerIcon }).bindPopup(spotPopup).addTo(spotMarkers)
    }
}