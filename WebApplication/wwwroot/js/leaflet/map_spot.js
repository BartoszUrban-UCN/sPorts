var mymap;
mymap = initMap('map');
mymap.setView([56.2639, 9.5018], 7);

function renderSpotMarker(lat, long, available) {
    if (!lat || !long) {
        return L.marker();
    }
    return renderMarker(lat, long, mymap, available)
}

function onMapClick(e) {
    currentMarker
        .setLatLng(e.latlng)
        .addTo(mymap)
        .bindPopup(`<b>Selected Location</b><br /> ${e.latlng.toString()} <br /><b>Double Click on marker to remove it</b>`).openPopup();

    latitude.val(e.latlng.lat);
    longitude.val(e.latlng.lng);
}

function removeMarker(e) {
    currentMarker.removeFrom(mymap);

    latitude.val(null);
    longitude.val(null);
}
