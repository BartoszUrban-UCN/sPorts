var mymap;
mymap = initMap('map');
mymap.setView([56.2639, 9.5018], 7);

function renderMarinas(marinas) {
    marinas.forEach(marina => {
        if (marina.Location) {
            renderCircle(marina.Location, mymap);
        }
    })
}