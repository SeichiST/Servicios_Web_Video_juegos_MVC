using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Servicios_Web_Video_juegos_MVC.Filters;
using Servicios_Web_Video_juegos_MVC.Helper;
using Servicios_Web_Video_juegos_MVC.Models;
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> GenerarVenta()
        {
            int? idClienteObj = HttpContext.Session.GetInt32("IdCliente");
            if (idClienteObj == null || idClienteObj == 0)
            {
                var clienteJson = HttpContext.Session.GetString("DatosClienteJson");
                if (!string.IsNullOrWhiteSpace(clienteJson) && clienteJson != "undefined")
                {
                    try
                    {
                        var json = JObject.Parse(clienteJson);
                        idClienteObj = (int?)json["idCliente"] ?? (int?)json["IdCliente"];
                    }
                    catch { }
                }
            }

            if (idClienteObj == null || idClienteObj == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var carrito = CarritoHelper.ObtenerCarrito(HttpContext.Session);

            if (!carrito.Any())
            {
                TempData["mensaje"] = "El carrito está vacío.";
                return RedirectToAction("VerCarrito");
            }

            var request = new
            {
                IdCliente = idClienteObj.Value,
                Detalles = carrito.Select(item => new
                {
                    IdJuegos = item.IdJuego,
                    Cantidad = item.Cantidad,
                    Precio = item.Precio
                }).ToList()
            };

            using (HttpClient cliente = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(request);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var respuesta = await cliente.PostAsync(
                    "https://localhost:7017/api/VentasAPI/RegistrarVenta", contenido);

                string resultado = await respuesta.Content.ReadAsStringAsync();

                if (!respuesta.IsSuccessStatusCode)
                {
                    TempData["mensaje"] = $"Error al registrar la venta: {resultado}";
                    return RedirectToAction("VerCarrito");
                }

                CarritoHelper.GuardarCarrito(HttpContext.Session, new List<CarritoItem>());
                TempData["mensaje"] = "Tu compra se registró correctamente. ¡Gracias por tu pedido!";
                TempData["exito"] = "1";
            }

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
