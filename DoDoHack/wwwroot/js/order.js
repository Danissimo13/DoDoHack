$(function () {
    var map = null;

    var query = `country=Россия&county=Брянская область&city=Брянск&street=${$('#street').val()}`;
    var path = `https://nominatim.openstreetmap.org/search?${query}&format=json&limit=1`;
    console.log(path);
    fetch(path)
        .then(function (response) {
            response.json().then(function (value) {
                console.log(value);
                const { display_name, lat, lon } = value[0];
                const position = [lat, lon]
                getMap(position, display_name);
            })
        });

    function getMap(position, message) {
        if (map == null) {
            map = L.map("map").setView(position, 18);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
                {
                    attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                }).addTo(map)
        }

        L.marker(position).addTo(map).bindPopup(message).openPopup();
    }
});