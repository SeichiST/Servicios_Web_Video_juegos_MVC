using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Servicios_Web_Video_juegos_MVC.Filters;
using Servicios_Web_Video_juegos_MVC.Helpers;
using Servicios_Web_Video_juegos_MVC.Models;
using System.Text;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    [Route("producto")]
    [LoginFilter]
    public class ProductoController : Controller
    {
        private readonly string _apiJuegos = "https://localhost:7017/api/JuegosAPI";
        private readonly string _apiCategorias = "https://localhost:7017/api/CategoriasAPI";
        private readonly string _apiMensajes = "https://localhost:7017/api/MensajesAPI";

        [HttpGet("inicio")]
        public IActionResult Inicio()
        {
            return View();
        }

        [HttpGet("nosotros")]
        public IActionResult Nosotros()
        {
            return View();
        }

        [HttpGet("contactanos")]
        public IActionResult Contactanos() {
            var model = new ContactoViewModel();

            var clienteJson = HttpContext.Session.GetString("DatosClienteJson");

            if (!string.IsNullOrWhiteSpace(clienteJson) && clienteJson != "undefined") {
                try {
                    var json = JObject.Parse(clienteJson);

                    string nombres = (string)json["nombres"] ?? (string)json["Nombres"] ?? "";
                    string apellidos = (string)json["apellidos"] ?? (string)json["Apellidos"] ?? "";
                    string correo = (string)json["correo"] ?? (string)json["Correo"] ?? "";

                    model.Nombre = $"{nombres} {apellidos}".Trim();
                    model.Correo = correo;
                }
                catch {
                    model.Correo = HttpContext.Session.GetString("UsuarioLogueado") ?? "";
                }
            }
            else {
                model.Correo = HttpContext.Session.GetString("UsuarioLogueado") ?? "";
            }

            return View(model);
        }

        [HttpPost("contactanos")]
        public async Task<IActionResult> Contactanos(ContactoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int idCliente = 0;
                var clienteJson = HttpContext.Session.GetString("DatosClienteJson");

                if (!string.IsNullOrWhiteSpace(clienteJson) && clienteJson != "undefined")
                {
                    var json = JObject.Parse(clienteJson);
                    idCliente = (int?)json["idCliente"] ?? (int?)json["IdCliente"] ?? 0;
                }

                var nuevoMensaje = new Mensaje
                {
                    IdCliente = idCliente,
                    TextoMensaje = model.Mensaje,
                    FechaEnvio = DateTime.Now,
                    Estado = "1"
                };

                using (HttpClient clienteHttp = new HttpClient())
                {
                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(nuevoMensaje),
                        Encoding.UTF8,
                        "application/json");

                    var rpta = await clienteHttp.PostAsync(_apiMensajes, contenido);

                    if (!rpta.IsSuccessStatusCode)
                    {
                        string error = await rpta.Content.ReadAsStringAsync();
                        TempData["MensajeError"] = $"No se pudo guardar el mensaje: {error}";
                        return View(model);
                    }
                }

                // 4. Enviar correo de confirmación (opcional / transaccional)
                try
                {
                    string cuerpoHtml = EmailHelper.GenerarPlantillaConfirmacion(model.Nombre, model.Mensaje);
                    await EmailHelper.EnviarCorreoAsync(model.Correo, "Confirmación de mensaje recibido - Soporte", cuerpoHtml);
                }
                catch
                { }

                TempData["MensajeExito"] = "¡Gracias por contactarnos! Tu mensaje fue registrado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al procesar la solicitud: {ex.Message}";
            }

            return RedirectToAction("Contactanos");
        }

        [HttpGet("juegos")]
        public async Task<IActionResult> Juegos(string? idCategoria, string? nombre, int? page)
        {
            var listadoJuegos = new List<Juego>();
            var listadoCategorias = new List<Categoria>();

            using (HttpClient cliente = new HttpClient())
            {
                var rptaJuegos = await cliente.GetAsync($"{_apiJuegos}/GetJuegos");
                if (rptaJuegos.IsSuccessStatusCode)
                {
                    string contenidoJuegos = await rptaJuegos.Content.ReadAsStringAsync();
                    var descompuestoJuegos = JsonConvert.DeserializeObject<List<Juego>>(contenidoJuegos);

                    if (descompuestoJuegos != null)
                    {
                        listadoJuegos = descompuestoJuegos.Where(j => j.Activo).ToList();
                    }
                }

                var rptaCat = await cliente.GetAsync($"{_apiCategorias}/GetCategorias");
                if (rptaCat.IsSuccessStatusCode)
                {
                    string contenidoCat = await rptaCat.Content.ReadAsStringAsync();
                    var descompuestoCat = JsonConvert.DeserializeObject<List<Categoria>>(contenidoCat);

                    if (descompuestoCat != null)
                    {
                        listadoCategorias = descompuestoCat;
                    }
                }
            }
            if (!string.IsNullOrEmpty(idCategoria))
            {
                listadoJuegos = listadoJuegos.Where(j => j.IdCategoria == idCategoria).ToList();
            }
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                listadoJuegos = listadoJuegos
                    .Where(j => j.Descripcion.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.LstCategoria = listadoCategorias;
            ViewBag.IdCategoriaSel = idCategoria;
            ViewBag.NombreBusqueda = nombre;

            var pagedList = PaginacioHelper.PaginarLista(listadoJuegos, page, 9);
            return View(pagedList);
        }

        [HttpGet("detalles_juego")]
        public async Task<IActionResult> DetallesJuego(int id)
        {
            var juego = new Juego();

            using (HttpClient cliente = new HttpClient())
            {
                var rpta = await cliente.GetAsync($"{_apiJuegos}/GetJuego/{id}");
                string contenido = await rpta.Content.ReadAsStringAsync();
                var descompuesto = JsonConvert.DeserializeObject<Juego>(contenido);

                if (descompuesto != null)
                {
                    juego = descompuesto;
                }
            }

            return View("detalles_juego",juego);
        }

       
    }
}