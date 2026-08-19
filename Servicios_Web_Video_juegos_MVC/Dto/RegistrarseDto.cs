namespace Servicios_Web_Video_juegos_MVC.Dto
{
    public class RegistrarseDto
    {
        public string Apellidos { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public DateOnly FechaNacimiento { get; set; }
        public string Sexo { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
