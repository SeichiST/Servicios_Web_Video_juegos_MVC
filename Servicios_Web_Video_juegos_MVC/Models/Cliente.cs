using System;
using System.Collections.Generic;

namespace Servicios_Web_Video_juegos_MVC.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Apellidos { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Sexo { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Estado { get; set; } = null!;

}
