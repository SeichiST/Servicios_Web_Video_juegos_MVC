using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicios_Web_Video_juegos_MVC.Dto;
using Servicios_Web_Video_juegos_MVC.Filters;
using Servicios_Web_Video_juegos_MVC.Models;
using System.Text;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    [Route("admin")]
    [AdminFilter]
    public class AdminController : Controller
    {
        private readonly string _apiClientes = "https://localhost:7017/api/ClientesAPI";

        [HttpGet("")]
        [HttpGet("index")]
        public IActionResult Index() {
            return RedirectToAction("ListaUsuarios");
        }

        [HttpGet("lista-usuarios")]
        public async Task<IActionResult> ListaUsuarios(string? estado) {
            var listado = new List<Cliente>();

            using (HttpClient cliente = new HttpClient()) {
                var rpta = await cliente.GetAsync($"{_apiClientes}/GetClientes");
                string contenido = await rpta.Content.ReadAsStringAsync();
                var descompuesto = JsonConvert.DeserializeObject<List<Cliente>>(contenido);

                if (descompuesto != null) {
                    listado = descompuesto;
                }
            }

            // Se carga la lista completa una sola vez y se filtra localmente,
            // sin volver a consultar la BD por cada búsqueda.
            if (!string.IsNullOrEmpty(estado)) {
                listado = listado.Where(c => c.Estado == estado).ToList();
            }

            ViewBag.EstadoSel = estado;
            return View(listado);
        }

        [HttpGet("registrar-usuario")]
        public IActionResult RegistrarUsuario() {
            return View(new Cliente { Estado = "1" });
        }

        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> RegistrarUsuario(Cliente cliente) {
            using (HttpClient httpClient = new HttpClient()) {
                string json = JsonConvert.SerializeObject(cliente);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var rpta = await httpClient.PostAsync(_apiClientes, contenido);

                if (!rpta.IsSuccessStatusCode) {
                    string error = await rpta.Content.ReadAsStringAsync();
                    ViewBag.Mensaje = $"No se pudo registrar el usuario: {error}";
                    return View(cliente);
                }
            }

            TempData["Mensaje"] = "Usuario registrado correctamente";
            return RedirectToAction("ListaUsuarios");
        }

        [HttpGet("modificar-usuario/{id}")]
        public async Task<IActionResult> ModificarUsuario(int id) {
            Cliente? cliente = null;

            using (HttpClient httpClient = new HttpClient()) {
                var rpta = await httpClient.GetAsync($"{_apiClientes}/GetCliente/{id}");

                if (!rpta.IsSuccessStatusCode) {
                    return NotFound();
                }

                string contenido = await rpta.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Cliente>(contenido);
            }

            if (cliente == null) {
                return NotFound();
            }

            return View(cliente);
        }

        // 2. POST: Recibe los cambios del formulario
        [HttpPost("modificar-usuario/{id}")]
        public async Task<IActionResult> ModificarUsuario(int id, Cliente cliente) {
            // Limpiamos referencias circulares/navegables antes de serializar
            //cliente.IdRols = null;
            //cliente.Mensajes = null;
            //cliente.Venta = null;
            cliente.Password = string.Empty;

            using (HttpClient httpClient = new HttpClient()) {
                string json = JsonConvert.SerializeObject(cliente);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviamos el PUT a la API
                var rpta = await httpClient.PutAsync(_apiClientes, contenido);

                if (!rpta.IsSuccessStatusCode) {
                    string error = await rpta.Content.ReadAsStringAsync();
                    ViewBag.Mensaje = $"No se pudo actualizar el usuario: {error}";
                    return View(cliente);
                }
            }

            TempData["Mensaje"] = "Usuario actualizado correctamente";
            return RedirectToAction("ListaUsuarios");
        }
    }
}