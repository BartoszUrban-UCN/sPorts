/**
 * Initializes an OST tiled map
 * @param {string} mapid - the id of the div for the map
 * @returns {L.Map} - leaflet map object
 */
function initMap(mapid) {
	var mymap = L.map(`${mapid}`)

	L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
		attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
		maxZoom: 18,
		id: 'mapbox/streets-v11',
		tileSize: 512,
		zoomOffset: -1,
		accessToken: 'pk.eyJ1IjoiemFja2Zyb3N0IiwiYSI6ImNraGRncmF2dTA1cDcyc2w2dGhua3RwOHoifQ.NKvsgUWbNOT14E-72mHn0w'
	}).addTo(mymap);

	var unavailableMarkerIcon = new L.Icon.Default();
	unavailableMarkerIcon.options.iconUrl = "marker-icon-unavailable.png";
	unavailableMarkerIcon.options.iconRetinaUrl = "marker-icon-unavailable-2x.png";

	return mymap;
}