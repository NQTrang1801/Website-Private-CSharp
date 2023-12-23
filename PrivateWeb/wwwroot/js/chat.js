const connection = new signalR.HubConnectionBuilder()
	.withUrl("/chatHub") // Đường dẫn đến hub
	.configureLogging(signalR.LogLevel.Information)
	.build();

$('#btnSendMessage').prop('disabled', true);

connection.start().then(function () {
	$('#btnSendMessage').prop('disabled', false);
	alert('Connected to ChatHub');
});

$('#btnSendMessage').click(function (e) {
	var user = $("#txtUser").val();
	var message = $("#txtMessage").val();
	connection.invoke("SendMessageToAll", user, message);
	// clear message input
	$("#txtMessage").val('');

	// focus again to textbox
	$("#txtMessage").focus();

	e.preventDefault();
});

connection.on("ReceiveMessage", function (user, message) {
	var content = `<b>${user} - </b>${message}`;
	$('#messagesList').append(`<li>${content}</li>`);
});