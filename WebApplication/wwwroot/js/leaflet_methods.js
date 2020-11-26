function MarinaBookingPrompt(marina, numberOfAvailableSpots) {
    var marinaPopup = `\
        <div class="text-center" style="width: 130px">\
            <p style="font-weight: bold; color: #737373">${marina.Name}</p>\
            <p style="font-weight: bold; color: #376186">${numberOfAvailableSpots} spots available!</p>\
            <button id="bookButton" class="btn btn-primary btn-sm">Book</button>\
        </div>\
    `;

    L.circle([parseFloat(marina.Location.Latitude), parseFloat(marina.Location.Longitude)], {
        color: 'pink',
        fillColor: '#f03',
        fillOpacity: 0.5,
        radius: ((parseFloat(marina.Location.Radius) + 0.1) * 1000),
    }).bindPopup(marinaPopup).addTo(markers);
}