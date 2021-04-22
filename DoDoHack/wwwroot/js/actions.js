$(function () {
    var userId = undefined;
    var messagesBox = $("#messages");
    var skipMessages = 0;
    var prevScrollH = 0;
    var endOfMessages = false;

    var hub = new signalR.HubConnectionBuilder()
        .withUrl("/courierchat")
        .build();

    hub.on("Enter", function (id) {
        userId = Number.parseInt(id);
    });

    hub.on("NewMessage", function (message) {
        console.log(messagesBox.prop("scrollHeight") - messagesBox.prop("scrollTop"));
        var willScroll = (messagesBox.prop("scrollHeight") - messagesBox.prop("scrollTop")) < (messagesBox.height() + 10);

        if (message.senderId.toString() == userId)
            messagesBox.append(CreateOwnMessageBlock(message));
        else
            messagesBox.append(CreateOutMessageBlock(message));

        if (willScroll) {
            messagesBox.prop("scrollTop", messagesBox.prop("scrollHeight"));
        }

        prevScrollH = messagesBox.prop("scrollHeight");
    });

    hub.on("LoadMessages", function (messages) {
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
    });

    hub.start().then(function () {
        hub.invoke("LoadMessages", skipMessages).then(function () {
            messagesBox.prop("scrollTop", messagesBox.prop("scrollHeight"));
            prevScrollH = messagesBox.prop("scrollHeight");
        });
    });

    messagesBox.on("scroll", function (e) {
        if (endOfMessages) return;

        var $that = $(e.target);
        if ($that.prop("scrollTop") <= 0) {
            hub.invoke("LoadMessages", skipMessages).then(function () {
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

        hub.invoke("SendMessage", message).then(function () {
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