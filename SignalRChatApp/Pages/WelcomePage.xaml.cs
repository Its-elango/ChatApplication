using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatApp.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly HubConnection hubConnection;

    public WelcomePage()
	{
		InitializeComponent();
        var baseUrl = "http://localhost";
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            baseUrl = "http://10.0.2.2";
        }
        hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}:5255/chatHub")
            .Build();
        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
           
        });
        Task.Run(async () =>
        {
            await hubConnection.StartAsync();
        });
    }

    private void joinBtn_Clicked(object sender, EventArgs e)
    {

    }
}