/// <reference path="jquery-3.4.1.min.js" />
var ws = null;
$(function () {
    if ("WebSocket" in window) {
        // 打开一个 web socket
        ws = new WebSocket("ws://localhost:7759");
        ws.onopen = function () {
        };

        ws.onmessage = function (evt) {
            var received_msg = evt.data;
            console.log(received_msg)
            //alert("数据已接收...");
            $("#messagehistory").append('<li><strong>接收：</strong>&nbsp;&nbsp;' + received_msg + '</li>');
        };

    }

    else {
        // 浏览器不支持 WebSocket
        alert("您的浏览器不支持 WebSocket!");
    }

})

function SendMessage(data) {
    ws.send(data);
}

function CloseWebsocket() {
    ws.onclose = function () {
        // 关闭 websocket
        alert("连接已关闭...");
    };
}
