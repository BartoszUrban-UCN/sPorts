var mymap = L.map('mapid').setView([51.505, -0.09], 13);

L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
	attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
	maxZoom: 18,
	id: 'mapbox/streets-v11',
	tileSize: 512,
	zoomOffset: -1,
	accessToken: 'pk.eyJ1IjoiemFja2Zyb3N0IiwiYSI6ImNraGRncmF2dTA1cDcyc2w2dGhua3RwOHoifQ.NKvsgUWbNOT14E-72mHn0w'
}).addTo(mymap);

var marker = L.marker([51.5, -0.09]).addTo(mymap);

var circle = L.circle([51.508, -0.11], {
	color: 'red',
	fillColor: '#f03',
	fillOpacity: 0.5,
	radius: 500
}).addTo(mymap);

var popup = L.popup();

var currentMarker = L.marker();

function onMapClick(e) {
	//popup
	//	.setLatLng(e.latlng)
	//	.setContent("You clicked the map at " + e.latlng.toString())
	//	.openOn(mymap);
	currentMarker
		.setLatLng(e.latlng)
		.addTo(mymap)
		.bindPopup("<b>Selected Location</b><br/>" + e.latlng.toString()).openPopup();
}

mymap.on('click', onMapClick);