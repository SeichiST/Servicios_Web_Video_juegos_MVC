using System;
using System.Collections.Generic;

namespace Servicios_Web_Video_juegos_MVC.Models;

public partial class Mensaje
{
    public int IdMensaje { get; set; }

    public int IdCliente { get; set; }

    public string TextoMensaje { get; set; } = null!;

    public DateTime FechaEnvio { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
    public virtual Cliente? Cliente { get; set; }
}
