namespace Servicios_Web_Video_juegos_MVC.Dto
{
    public class VentaResponseDto
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public List<DetalleResponseDto> Detalles { get; set; } = new();
    }

    public class DetalleResponseDto
    {
        public int IdJuegos { get; set; }
        public string NombreJuego { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}