using Microsoft.AspNetCore.Mvc;
using Servicios_Web_Video_juegos_MVC.Filters;
using Servicios_Web_Video_juegos_MVC.Helper;
using Servicios_Web_Video_juegos_MVC.Models;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    [LoginFilter]
    public class CarritoController : Controller
    {
        
        [HttpPost]
        public IActionResult AgregarCarrito(int idJuego, string descripcion, string imagen, decimal precio, int cantidad)
        {
            var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);

            var item = carrito.FirstOrDefault(c => c.IdJuego == idJuego);

            if (item != null)
            {
                item.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdJuego = idJuego,
                    Descripcion = descripcion,
                    Imagen = imagen,
                    Precio = precio,
                    Cantidad = cantidad
                });
            }

            CarritoHelper.GuardarCarrito(HttpContext.Session, carrito);

            return RedirectToAction("VerCarrito");
        }

       
        [HttpGet]
        public IActionResult VerCarrito()
        {
            var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);
            return View(carrito);
        }
    }
}
