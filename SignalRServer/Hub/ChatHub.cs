using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        //public async Task SendMessageToAll(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        public async Task SendMessageToAll(string user, string message, string? imageData)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message, imageData);
            }
            catch(Exception ex)
            { 
                Debug.WriteLine(ex.Message ,ex.StackTrace);
            }
           
           
        }
    }
}