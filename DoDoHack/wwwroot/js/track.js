$(function () {
    var courierId = Number.parseInt($("#courier-id").val());
    var map = null;
    var lastMarker = null;
    var saveTrackOnMap = false;
    var trackNumber = 0;

    $("#no-tracking").hide();

    $("#change-track-save-btn").on("click", function () {
        saveTrackOnMap = !saveTrackOnMap;
        if (saveTrackOnMap) {
            $("#change-track-save-btn").text("Менять треки на карте")
        }
        else {
            $("#change-track-save-btn").text("Сохранять треки на карте")
        }
    })

    var hub = new signalR.HubConnectionBuilder()
        .withUrl("/couriertrack")
        .build();

    hub.on("GetTrack", function (_courierId, coordinates) {
        if (courierId != _courierId) return;

        console.log(`Got coordinate from ${_courierId} : ${coordinates.longitude} ${coordinates.latitude}`);

        $("#no-tracking").hide();
        const position = [coordinates.latitude, coordinates.longitude];
        getMap(position, `Трек №${trackNumber}`);
        trackNumber++;
    });

    hub.on("NoTracking", function (_courierId) {
        if (courierId != _courierId) return;
        $("#no-tracking").show();
    });

    hub.start().then(function () {
        hub.invoke("GetTrack", courierId);
        setInterval(function () {
            hub.invoke("GetTrack", courierId);
        }, 10000);
    });;

    function getMap(position, message) {
        if (map == null) {
            map = L.map("map").setView(position, 18);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
                {
                    attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                }).addTo(map)
        }
        console.log(trackNumber, message);

        if (saveTrackOnMap) {
            lastMarker = L.marker(position).addTo(map).bindPopup(message).openPopup();
        }
        else {
            if (lastMarker) {
                map.removeLayer(lastMarker)
            }
            lastMarker = L.marker(position).addTo(map).bindPopup(message).openPopup();
        }
    }
})