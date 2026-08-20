using Microsoft.AspNetCore.SignalR;
namespace Servicios_Web_Video_juegos_MVC.Hubs
{
    public class ChatHub : Hub
    {
        public async Task EnviarMensaje(string remitente, string texto)
        {
            await Clients.All.SendAsync("RecibirMensaje", remitente, texto, DateTime.Now.ToString("HH:mm"));
        }
    }
}
