using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly string _apiCategorias = "https://localhost:7017/api/CategoriasAPI";
        private readonly string _apiJuegos = "https://localhost:7017/api/JuegosAPI";
        private readonly string _apiMensajes = "https://localhost:7017/api/MensajesAPI";
        private readonly IWebHostEnvironment _env;


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

        private async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            var listado = new List<Categoria>();
            using (HttpClient cliente = new HttpClient())
            {
                var rpta = await cliente.GetAsync($"{_apiCategorias}/GetCategorias");
                if (rpta.IsSuccessStatusCode)
                {
                    string contenido = await rpta.Content.ReadAsStringAsync();
                    listado = JsonConvert.DeserializeObject<List<Categoria>>(contenido) ?? new List<Categoria>();
                }
            }
            return listado;
        }

        public AdminController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("lista-juegos")]
        public async Task<IActionResult> ListaJuegos(string? idCategoria)
        {
            var listado = new List<Juego>();

            using (HttpClient cliente = new HttpClient())
            {
                var rpta = await cliente.GetAsync($"{_apiJuegos}/GetJuegos");
                if (rpta.IsSuccessStatusCode)
                {
                    string contenido = await rpta.Content.ReadAsStringAsync();
                    listado = JsonConvert.DeserializeObject<List<Juego>>(contenido) ?? new List<Juego>();
                }
            }

            // 1. Obtenemos todas las categorías
            var categorias = await ObtenerCategoriasAsync();

            foreach (var juego in listado)
            {
                juego.Categoria = categorias.FirstOrDefault(c => c.IdCategoria == juego.IdCategoria);
            }

            // 3. Aplicamos el filtro si seleccionaron una categoría
            if (!string.IsNullOrEmpty(idCategoria))
            {
                listado = listado.Where(j => j.IdCategoria == idCategoria).ToList();
            }

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion", idCategoria);

            return View("lista-juegos", listado);
        }

        [HttpGet("registrar-juego")]
        public async Task<IActionResult> RegistrarJuego()
        {
            var categorias = await ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion");
            return View("registrar-juego", new Juego());
        }

        [HttpPost("registrar-juego")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarJuego(Juego obj, IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "ImgJuegos");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    int siguienteNumero = 1;
                    var archivos = Directory.GetFiles(uploadsFolder);
                    var numeros = archivos
                        .Select(f => Path.GetFileNameWithoutExtension(f))
                        .Where(f => int.TryParse(f, out _))
                        .Select(int.Parse);

                    if (numeros.Any())
                    {
                        siguienteNumero = numeros.Max() + 1;
                    }

                    string extension = Path.GetExtension(file.FileName);
                    string uniqueFileName = $"{siguienteNumero:D5}{extension}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    obj.Imagen = uniqueFileName;
                }

                obj.Activo = true;

                using (HttpClient cliente = new HttpClient())
                {
                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(obj),
                        Encoding.UTF8,
                        "application/json");

                    var rpta = await cliente.PostAsync(_apiJuegos, contenido);

                    if (rpta.IsSuccessStatusCode)
                    {
                        TempData["Mensaje"] = await rpta.Content.ReadAsStringAsync();
                        return RedirectToAction("ListaJuegos");
                    }

                    ViewBag.Mensaje = await rpta.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
            }

            var categorias = await ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion", obj.IdCategoria);
            return View("registrar-juego", obj);
        }

        [HttpGet("modificar-juego/{id}")]
        public async Task<IActionResult> ModificarJuego(int id)
        {
            Juego? obj = null;

            using (HttpClient cliente = new HttpClient())
            {
                var rpta = await cliente.GetAsync($"{_apiJuegos}/GetJuego/{id}");
                if (rpta.IsSuccessStatusCode)
                {
                    string contenido = await rpta.Content.ReadAsStringAsync();
                    obj = JsonConvert.DeserializeObject<Juego>(contenido);
                }
            }

            if (obj == null) return NotFound();

            var categorias = await ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion", obj.IdCategoria);

            return View("modificar-juego", obj);
        }

        [HttpPost("modificar-juego/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModificarJuego(int id, Juego obj, IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "ImgJuegos");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    obj.Imagen = uniqueFileName;
                }

                using (HttpClient cliente = new HttpClient())
                {
                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(obj),
                        Encoding.UTF8,
                        "application/json");

                    var rpta = await cliente.PutAsync(_apiJuegos, contenido);

                    if (rpta.IsSuccessStatusCode)
                    {
                        TempData["Mensaje"] = await rpta.Content.ReadAsStringAsync();
                        return RedirectToAction("ListaJuegos");
                    }

                    ViewBag.Mensaje = await rpta.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
            }

            var categorias = await ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Descripcion", obj.IdCategoria);
            return View("modificar-juego", obj);
        }

        [HttpGet("lista-mensajes")]
        public async Task<IActionResult> ListaMensajes()
        {
            var listadoMensajes = new List<Mensaje>();
            var listadoClientes = new List<Cliente>();

            using (HttpClient cliente = new HttpClient())
            {
                var rptaMensajes = await cliente.GetAsync($"{_apiMensajes}/GetMensajes");
                if (rptaMensajes.IsSuccessStatusCode)
                {
                    string contenido = await rptaMensajes.Content.ReadAsStringAsync();
                    listadoMensajes = JsonConvert.DeserializeObject<List<Mensaje>>(contenido) ?? new List<Mensaje>();
                }

                var rptaClientes = await cliente.GetAsync($"{_apiClientes}/GetClientes");
                if (rptaClientes.IsSuccessStatusCode)
                {
                    string contenido = await rptaClientes.Content.ReadAsStringAsync();
                    listadoClientes = JsonConvert.DeserializeObject<List<Cliente>>(contenido) ?? new List<Cliente>();
                }
            }

            foreach (var msg in listadoMensajes)
            {
                msg.Cliente = listadoClientes.FirstOrDefault(c => c.IdCliente == msg.IdCliente);
            }

            return View("lista-mensajes", listadoMensajes.OrderByDescending(m => m.FechaEnvio).ToList());
        }

    }
}