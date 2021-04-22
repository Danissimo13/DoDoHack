$(function () {
    var userId = undefined;
    var partnerId = Number.parseInt(location.pathname.substr(location.pathname.lastIndexOf('/') + 1, location.pathname.length - location.pathname.lastIndexOf('/')));

    var messagesBox = $("#messages");
    var skipMessages = 0;
    var prevScrollH = 0;
    var endOfMessages = false;

    var hub = new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .build();

    hub.on("Enter", function (id) {
        userId = id;
    });

    hub.on("NewMessage", function (message, senderId, receiverId) {
        if (((senderId == userId) && (receiverId == partnerId)) || ((senderId == partnerId) && (receiverId == userId))) {
            var willScroll = (messagesBox.prop("scrollHeight") - messagesBox.prop("scrollTop")) < (messagesBox.height() + 10);

            if (message.senderId.toString() == userId)
                messagesBox.append(CreateOwnMessageBlock(message));
            else
                messagesBox.append(CreateOutMessageBlock(message));

            if (willScroll) {
                messagesBox.prop("scrollTop", messagesBox.prop("scrollHeight"));
            }

            prevScrollH = messagesBox.prop("scrollHeight");
        }
    });

    hub.on("LoadMessages", function (messages, senderId, receiverId) {
        console.log(senderId, receiverId);
        console.log(userId, partnerId);
        if (((senderId == userId) && (receiverId == partnerId)) || ((senderId == partnerId) && (receiverId == userId))) {

            if (messages.length == 0) {
                endOfMessages = true;
            }

            skipMessages += messages.length;
            for (message of messages) {
                if (message.senderId.toString() == userId)
                    messagesBox.prepend(CreateOwnMessageBlock(message));
                else
                    messagesBox.prepend(CreateOutMessageBlock(message));
            }
        }
    });

    hub.start().then(function () {
        hub.invoke("LoadMessages", partnerId, skipMessages).then(function () {
            messagesBox.prop("scrollTop", messagesBox.prop("scrollHeight"));
            prevScrollH = messagesBox.prop("scrollHeight");
        });
    });

    messagesBox.on("scroll", function (e) {
        if (endOfMessages) return;

        var $that = $(e.target);
        if ($that.prop("scrollTop") <= 0) {
            hub.invoke("LoadMessages", partnerId, skipMessages).then(function () {
                $that.scrollTop($that.prop('scrollHeight') - prevScrollH);
                prevScrollH = $that.prop('scrollHeight');
            });
        }
    });

    $('#message-text').on("keyup", function (e) {
        if (e.code == "Enter") {
            $("#send-message").trigger("click");
        }
    });

    $("#send-message").on("click", function () {
        var message = $("#message-text").val();

        if (message.trim() == "") return;

        hub.invoke("SendMessage", partnerId, message).then(function () {
            $("#message-text").val("");
        });
    });

    function CreateOutMessageBlock(message) {
        var messageHtml = `<div class="message out">${message.message}</div>`;
        return $(messageHtml);
    }

    function CreateOwnMessageBlock(message) {
        var messageHtml = `<div class="message own">${message.message}</div>`;
        return $(messageHtml);
    }
});