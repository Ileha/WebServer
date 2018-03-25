var socket;
function connect() {
  socket = new WebSocket("ws://localhost:11001");

  socket.onopen = function() {
    alert("Соединение установлено.");
  };

  socket.onclose = function(event) {
    if (event.wasClean) {
      alert('Соединение закрыто чисто');
    } else {
      alert('Обрыв соединения'); // например, "убит" процесс сервера
    }
    alert('Код: ' + event.code + ' причина: ' + event.reason);
  };

  socket.onmessage = function(event) {
    alert("Получены данные " + event.data);
  };

  socket.onerror = function(error) {
    alert("Ошибка " + error.message);
  };
}

// HTTP/1.1 101 Switching Protocols
// Server: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)
// Upgrade: websocket
// Connection: Upgrade
// Sec-WebSocket-Accept: Jf4b3fRySYKpUgx1/NZMz7CUTQc=
