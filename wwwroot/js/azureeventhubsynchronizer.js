"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/eventHub").build();

//Disable send button until connection is established
document.getElementById("retriveEventsFromAzure").disabled = true;

connection.on("ReceiveMessage", function (messagesFromAzureEventHub) {
    debugger;  
    document.getElementById("messagesList").innerText = messagesFromAzureEventHub;
});

connection.start().then(function () {
    debugger;
    document.getElementById("retriveEventsFromAzure").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("retriveEventsFromAzure").addEventListener("click", function (event) {
    debugger;
    connection.invoke("RetrieveMessagesFromAzure").catch(function (err) {
        debugger;
        return console.error(err.toString());
    });
    event.preventDefault();
});