$(function () {
    var courierId = Number.parseInt($("#courier-id").val());
    var map = null;

    $("#no-tracking").hide();

    document.getElementById('track-day').valueAsDate = new Date();
    $("#track-day").on("change", function () {
        var date = new Date(Date.parse($("#track-day").val()));
        hub.invoke("GetTracksByDate", courierId, date);
    })

    var hub = new signalR.HubConnectionBuilder()
        .withUrl("/couriertrack")
        .build();

    hub.on("GetTracksByDate", function (_courierId, tracks) {
        if (courierId != _courierId) return;

        console.log(tracks);
        if (tracks.length == 0) $("#no-tracking").show();
        else $("#no-tracking").hide();

        for (var i in tracks) {
            const position = [tracks[i].latitude, tracks[i].longitude];
            const time = tracks[i].trackTime.substr(0, tracks[i].trackTime.lastIndexOf("."))
            console.log(tracks[i].latitude, tracks[i].longitude, tracks[i].trackTime, time);

            getMap(position, new Date(Date.parse(time)).toTimeString());
        }
    })

    hub.on("NoTracking", function (_courierId) {
        if (courierId != _courierId) return;
        $("#no-tracking").show();
    });

    hub.start().then(function () {
        hub.invoke("GetTracksByDate", courierId, (new Date()));
    });;

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