﻿var socket;
function connect() {
    socket = new WebSocket("ws://");
    socket.onopen = function() {
        console.log("Соединение установлено.");
    };
    socket.onclose = function(event) {
        if (event.wasClean) {
            console.log("Соединение закрыто чисто");
        } else {
            console.log("Обрыв соединения");
        }
        console.log("Код: " + event.code + " причина: " + event.reason);
    };
    socket.onmessage = function(event) {
        console.log("Получены данные " + event.data);
    };
    socket.onerror = function(error) {
        console.log("Ошибка " + error.message);
    };
}