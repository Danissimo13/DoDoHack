$(function () {
    initSOS();
    initTracking();

    function initTracking() {
        var sosElem = document.getElementById("sos");
        if (!sosElem) return; // Сделать нормальную проверку на курьера а не это всё...

        var work = false;

        var hub = new signalR.HubConnectionBuilder()
            .withUrl("/couriertrack")
            .build();
        hub.start().then(function () {
            if (sessionStorage.getItem("isTracking")) {
                work = true;
                var timeToNext = 0;
                if (sessionStorage.getItem("lastTrack")) {
                    var lastTrack = Date.parse(sessionStorage.getItem("lastTrack"));
                    var now = new Date();

                    if (((now - lastTrack) / 1000) < 10) {
                        timeToNext = 10 - ((now - lastTrack) / 1000);
                        timeToNext *= 1000;
                    }
                }

                setTimeout(function () {
                    SendCoordinates();
                    StartTrackInterval();
                }, timeToNext);
            }
        });

        if (sessionStorage.getItem("isTracking")) {
            $("#turn-on-track").hide();
        }
        else {
            $("#turn-off-track").hide();
        }
        
        $("#turn-on-track").on("click", function () {
            if (sessionStorage.getItem("isTracking")) return;

            sessionStorage.setItem("isTracking", "true");
            StartTrackInterval();

            $("#turn-on-track").hide();
            $("#turn-off-track").show();
        });

        $("#turn-off-track").on("click", function () {
            sessionStorage.removeItem("lastTrack");
            sessionStorage.removeItem("isTracking");

            work = false;

            $("#turn-off-track").hide();
            $("#turn-on-track").show();
        });

        var interval = undefined;
        function StartTrackInterval() {
            work = true;
            interval = setInterval(SendCoordinates, 10000);
        }

        function SendCoordinates() {
            if (work) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    hub.invoke("UpdateTrack", { Longitude: position.coords.longitude, Latitude: position.coords.latitude });
                    console.log(`Send coordinates ${position.coords.longitude} ${position.coords.latitude}`);
                }, function () { }, { enableHighAccuracy: true });
            }
            else {
                clearInterval(interval);
                interval = undefined;
                hub.invoke("StopTrack");
            }

            sessionStorage.setItem("lastTrack", (new Date()).toUTCString());
        }
    }

    function initSOS() {
        var sosElem = document.getElementById("sos");
        if (!sosElem) return;

        var hub = new signalR.HubConnectionBuilder()
            .withUrl("/support")
            .build();

        hub.start();

        $("#sos").on("click", function () {
            $("#sos").addClass("called");
            $("#sos").prop("disabled", true);

            navigator.geolocation.getCurrentPosition(function (position) {
                console.log(position);
                if (position.coords.accuracy > 400) {
                    alert("Включите геолокацию на устройстве!")
                }

                var locationRef = `https://yandex.ru/maps/?ll=${position.coords.longitude}%2C${position.coords.latitude}&mode=routes&rtext=~${position.coords.latitude}%2C${position.coords.longitude}&rtt=auto&ruri=&source=wizgeo&utm_medium=maps-desktop&utm_source=serp&z=18`;
                console.log(locationRef);
                hub.invoke("SOS", locationRef);
            }, function () {
                alert("Разрешите получение координат для правильной работы!");
                hub.invoke("SOS", "");
            }, { enableHighAccuracy: true });
        });
    }
});