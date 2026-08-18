namespace Servicios_Web_Video_juegos_MVC.Dto
{
    public class RoleDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public List<RoleDto> IdRols { get; set; } = new List<RoleDto>();
    }
}