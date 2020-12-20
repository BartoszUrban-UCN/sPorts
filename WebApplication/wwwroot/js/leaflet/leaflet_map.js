/**
 * Initializes an OST tiled map
 * @param {string} mapid - the id of the div for the map
 * @returns {L.Map} - leaflet map object
 */
function initMap(mapid) {
	var mymap = L.map(`${mapid}`);

	L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
		attribution: 'Map data © <a href="https://www.mapbox.com/">Mapbox</a>',
		minZoom: 4,
		maxZoom: 21,
		id: 'mapbox/satellite-streets-v11',
		tileSize: 512,
		zoomOffset: -1,
		accessToken: 'pk.eyJ1IjoiemFja2Zyb3N0IiwiYSI6ImNraGRncmF2dTA1cDcyc2w2dGhua3RwOHoifQ.NKvsgUWbNOT14E-72mHn0w'
	}).addTo(mymap);

	this.unavailableSpotIcon = new L.Icon.Default();
	unavailableSpotIcon.options.iconUrl = "marker-icon-unavailable.png";
	unavailableSpotIcon.options.iconRetinaUrl = "marker-icon-unavailable-2x.png";

	return mymap;
}