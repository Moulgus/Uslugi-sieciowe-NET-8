"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7099/chatHub")
    .withAutomaticReconnect()
    .build();

const sendButton = document.getElementById("sendButton");
const userInput = document.getElementById("userInput");
const messageInput = document.getElementById("messageInput");
const messagesList = document.getElementById("messagesList");

sendButton.disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    const li = document.createElement("li");
    messagesList.appendChild(li);
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    sendButton.disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

sendButton.addEventListener("click", function (event) {
    const user = userInput.value.trim();
    const message = messageInput.value.trim();

    if (!user || !message) {
        event.preventDefault();
        return;
    }

    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    messageInput.value = "";
    messageInput.focus();
    event.preventDefault();
});
