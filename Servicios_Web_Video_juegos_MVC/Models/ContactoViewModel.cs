namespace Servicios_Web_Video_juegos_MVC.Models
{
    public class ContactoViewModel
    {
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}