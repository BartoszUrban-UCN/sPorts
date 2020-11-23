function AddMarinaOnMap(marina) {
    var marinaPopup = `\
        <div class="text-center" style="width: 100px">\
            <p style="font-weight: bold">${marina.Name}</p>\
            <a href="/Marina/Details/${marina.MarinaId}" class="btn btn-primary text-white">Details</a>\
        </div>\
    `;

    L.circle([marina.Location.Latitude, marina.Location.Longitude], {
        color: 'pink',
        fillColor: '#f03',
        fillOpacity: 0.5,
        radius: (marina.Location.Radius * 1000),
    }).bindPopup(marinaPopup).openTooltip().addTo(mymap);
}