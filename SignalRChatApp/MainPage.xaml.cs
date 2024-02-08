using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SignalRChatApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HubConnection hubConnection;
        public List<byte[]> ImageDataList { get; set; } = new List<byte[]>();

        private string _sender {  get; set; }

        public MainPage()
        {
            InitializeComponent();
            var baseUrl = "http://localhost";
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                baseUrl = "http://10.0.2.2";
            }
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}:5255/chatHub")
                .Build();
            hubConnection.On<string, string, string>("ReceiveMessage", (user, message, imageData) =>
            {
                Dispatcher.Dispatch(() =>
                {
                    if (!string.IsNullOrEmpty(imageData))
                    {
                        Label labelUser = new Label();
                        Label labelMessage = new Label();
                        Image image = new Image();

                        labelUser.Text = user;
                        labelMessage.Text = message;

                        var newImageData = Convert.FromBase64String(imageData);
                        ImageDataList.Add(newImageData); 

                        MemoryStream memoryStream = new MemoryStream(newImageData);
                        image.Source = ImageSource.FromStream(() => memoryStream);
                        image.HeightRequest = 100;
                        image.WidthRequest = 100;

                        if (_sender == user)
                        {
                            image.HorizontalOptions = image.VerticalOptions = LayoutOptions.EndAndExpand;
                            labelMessage.HorizontalOptions = labelMessage.VerticalOptions = LayoutOptions.EndAndExpand;

                        }
                        else
                        {
                            image.HorizontalOptions = image.VerticalOptions = LayoutOptions.StartAndExpand;
                            labelMessage.HorizontalOptions = labelMessage.VerticalOptions = LayoutOptions.StartAndExpand;

                        }
                        StackLayout stacklayout = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal
                        };

                        //stacklayout.Children.Add(labelUser);
                        stacklayout.Children.Add(labelMessage);
                        stacklayout.Children.Add(image);

                        lblchat.Children.Add(stacklayout);
                    }
                    else
                    {
                        Label labelUser = new Label();
                        Label labelMessage = new Label();
                        Image image = new Image();

                        labelUser.Text = user;
                        labelMessage.Text = message;

                        if (_sender == user)
                        {
                            labelMessage.HorizontalOptions = LayoutOptions.EndAndExpand;
                        }
                        else
                        {
                            labelMessage.HorizontalOptions = LayoutOptions.StartAndExpand;
                        }
                        StackLayout stacklayout = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal
                        };

                       // stacklayout.Children.Add(labelUser);
                        stacklayout.Children.Add(labelMessage);
                        stacklayout.Children.Add(image);
                        lblchat.Children.Add(stacklayout);
                    }
                });
            });
            Task.Run(async () =>
            {
                await hubConnection.StartAsync();
            });
        }

        private async void btnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    _sender = txtUsername.Text;
                    
                    await hubConnection.InvokeCoreAsync("SendMessageToAll", args: new[]
                    {
                        txtUsername.Text,
                        txtMessage.Text,
                        ImageDataList.Count > 0 ? Convert.ToBase64String(ImageDataList[ImageDataList.Count - 1]) : null                    });
                    txtMessage.Text = String.Empty;
                    ImageDataList.Clear();

                }
               

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void filePicker_Clicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image"
            });

            if (result != null)
            {
                using (var stream = await result.OpenReadAsync())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        var imageData = ms.ToArray();
                        ImageDataList.Add(imageData);
                    }
                }
            }
        }
    }
}
