/**
 * 
 * @param {location} location - Location model for the circle
 * @param {any} addTo - map or layer group object to add the circle to
 * @returns {L.Circle} - use this to bind further commands like binding a popup
 */
function renderCircle(location, addTo) {
    return L.circle([parseFloat(location.Latitude), parseFloat(location.Longitude)], {
        color: 'pink',
        fillColor: '#f03',
        fillOpacity: 0.5,
        radius: ((parseFloat(location.Radius) + 0.1) * 1000),
    }).addTo(addTo);
}

/**
 * 
 * @param {location} location - Location model for the circle
 * @param {any} addTo - map or layer group object to add the circle to
 * @param {boolean} [unavailable=false] - true to use a different icon
 * @returns {L.Marker} - use this to bind further commands like binding a popup
 */
function renderMarker(location, addTo, unavailable) {
    if (unavailable) {
        return L.marker([parseFloat(location.Latitude), parseFloat(location.Longitude)], { icon: unavailableMarkerIcon }).addTo(addTo);
    } else {
        return L.marker([parseFloat(location.Latitude), parseFloat(location.Longitude)]).addTo(addTo);
    }
}

/**
 * 
 * @param {number} lat - Latitude value
 * @param {number} long - Longitude value
 * @param {any} addTo - map or layer group object to add the circle to
 * @param {boolean} [unavailable=false] - true to use a different icon
 * @returns {L.Marker} - use this to bind further commands like binding a popup
 */
function renderMarker(lat, long, addTo, unavailable) {
    if (unavailable) {
        return L.marker([parseFloat(lat), parseFloat(long)], { icon: unavailableMarkerIcon }).addTo(addTo);
    } else {;
        return L.marker([parseFloat(lat), parseFloat(long)]).addTo(addTo);
    }
}