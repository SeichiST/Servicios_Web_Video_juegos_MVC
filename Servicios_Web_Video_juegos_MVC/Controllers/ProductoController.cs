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
        public async Task<IActionResult> Contactanos(ContactoViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            try {
                //Generar la plantilla HTML desde el Helper
                string cuerpoHtml = EmailHelper.GenerarPlantillaConfirmacion(model.Nombre, model.Mensaje);

                //Enviar el correo usando el Helper
                await EmailHelper.EnviarCorreoAsync(model.Correo, "Confirmación de mensaje recibido - Soporte", cuerpoHtml);

                TempData["MensajeExito"] = "¡Gracias por contactarnos! Se ha enviado un correo de confirmación.";
            }
            catch (Exception ex) {
                TempData["MensajeError"] = "Tu mensaje fue procesado, pero no se pudo enviar el correo de confirmación.";
            }

            return RedirectToAction("Contactanos");
        }

        [HttpGet("juegos")]
        public async Task<IActionResult> Juegos(string? idCategoria, int? page)
        {
            var listadoJuegos = new List<Juego>();
            var listadoCategorias = new List<Categoria>();

            using (HttpClient cliente = new HttpClient())
            {
                var rptaJuegos = await cliente.GetAsync($"{_apiJuegos}/GetJuegos");
                string contenidoJuegos = await rptaJuegos.Content.ReadAsStringAsync();
                var descompuestoJuegos = JsonConvert.DeserializeObject<List<Juego>>(contenidoJuegos);

                if (descompuestoJuegos != null)
                {
                    listadoJuegos = descompuestoJuegos
                        .Where(j => j.Activo)
                        .ToList();
                }

                var rptaCat = await cliente.GetAsync($"{_apiCategorias}/GetCategorias");
                string contenidoCat = await rptaCat.Content.ReadAsStringAsync();
                var descompuestoCat = JsonConvert.DeserializeObject<List<Categoria>>(contenidoCat);

                if (descompuestoCat != null)
                {
                    listadoCategorias = descompuestoCat;
                }
            }

            //categorias
            if (!string.IsNullOrEmpty(idCategoria))
            {
                listadoJuegos = listadoJuegos.Where(j => j.IdCategoria == idCategoria).ToList();
            }

            ViewBag.LstCategoria = listadoCategorias;
            ViewBag.IdCategoriaSel = idCategoria;

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