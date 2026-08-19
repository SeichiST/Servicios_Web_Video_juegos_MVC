using System;
using System.Collections.Generic;

namespace Servicios_Web_Video_juegos_MVC.Models;

public partial class Juego
{
    public int IdJuegos { get; set; }

    public string? IdCategoria { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public string Imagen { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual Categoria? Categoria { get; set; }

}
