var mymap;
mymap = initMap('map');
mymap.setView([56.2639, 9.5018], 7);

var marinaPopup = '\
    <div class="text-center" style="width: 200px">\
        <b>Selected Location</b>\
        <div style="display: flex; align-items: baseline; justify-content: space-evenly;">\
            <label class="control-label" style="padding-right: 10px;">Size</label>\
            <input id="popup-location-radius" class="form-control form-control-sm popup-input" type="number" style="margin: 10px 0px 10px 0px" placeholder="Radius" min="7" />\
        </div>\
        <button type="button" class="btn btn-primary btn-sm" onclick=removeCircleMarker()>Remove</button>\
    </div>\
';

function renderMarinaCircle(lat, long, radius) {
    if (!lat || !long) {
        return L.circle();
    }
    return renderCircle(lat, long, radius, mymap);
}

function onMapClick(e) {
    currentCircleMarker
        .setLatLng(e.latlng)
        .setStyle({
            color: 'cadetblue',
            fillColor: 'aquamarine',
            fillOpacity: 0.5,
        })
        .addTo(mymap)
        .bindPopup(marinaPopup)
        .openPopup();

    latitude.val(e.latlng.lat);
    longitude.val(e.latlng.lng);

    popupRadius = $("#popup-location-radius");
    popupRadius.on('change', updateLocationRadius);
    popupRadius.on('input', updateLocationRadius);

    popupRadius.val(10);

    updateLocationRadius();
}

function removeCircleMarker(e) {
    currentCircleMarker.removeFrom(mymap);

    latitude.val(null);
    longitude.val(null);
    radius.val(null);
}

function updateLocationRadius(e) {
    currentCircleMarker
        .setRadius(popupRadius.val() * 1000);
    radius.val(popupRadius.val());
}