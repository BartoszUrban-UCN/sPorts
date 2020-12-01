/**
 * Render circle for marina area from JSON in booking
 * @param {location} location - Location model for the circle
 * @param {any} addTo - map or layer group object to add the circle to
 * @returns {L.Circle} - use this to bind further commands like binding a popup
 */
function renderCircleLocation(location, addTo) {
    return renderCircle(location.Latitude, location.Longitude, location.Radius, addTo);
}

/**
 * Render circle for marina area
 * @param {number} lat - latitude of the center
 * @param {number} long - longitude of the center
 * @param {number} radius - radius for the circle
 * @param {any} addTo - map or layer group object to add the circle to
 * @returns {L.Circle} - use this to bind further commands like binding a popup
 */
function renderCircle(lat, long, radius, addTo) {
    return L.circle([parseFloat(lat), parseFloat(long)], {
        color: 'pink',
        fillColor: '#f03',
        fillOpacity: 0.5,
        radius: ((parseFloat(radius) + 0.1) * 1000),
    }).addTo(addTo);
}

/**
 * Render marker for spot location from JSON in booking
 * @param {location} location - Location model for the circle
 * @param {any} addTo - map or layer group object to add the circle to
 * @param {boolean} [available=true] - false to use a different icon
 * @returns {L.Marker} - use this to bind further commands like binding a popup
 */
function renderMarkerLocation(location, addTo, available) {
    return renderMarker(location.Latitude, location.Longitude, addTo, available)
}

/**
 * Render marker for spot location
 * @param {number} lat - Latitude value
 * @param {number} long - Longitude value
 * @param {any} addTo - map or layer group object to add the circle to
 * @param {boolean} [available=false] - false to use a different icon
 * @returns {L.Marker} - use this to bind further commands like binding a popup
 */
function renderMarker(lat, long, addTo, available) {
    if (available) {
        return L.marker([parseFloat(lat), parseFloat(long)]).addTo(addTo);
    } else {
        return L.marker([parseFloat(lat), parseFloat(long)], { icon: unavailableSpotIcon }).addTo(addTo);
    }
}