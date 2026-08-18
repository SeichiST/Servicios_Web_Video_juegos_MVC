using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicios_Web_Video_juegos_MVC.Dto;
using System.Text;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _apilogin =
            "https://localhost:7017/api/LoginAPI/Login";
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            using (HttpClient cliente = new HttpClient())
            {
                var login = new LoginRequestDto { Correo = correo, Password = password };
                string json = JsonConvert.SerializeObject(login);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var respuesta = await cliente.PostAsync(_apilogin, contenido);

                if (!respuesta.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Correo o contraseña incorrectos";
                    return View("Index");
                }

                // 1. Leemos el cuerpo de la respuesta de la API UNA SOLA VEZ
                string respuestaJson = await respuesta.Content.ReadAsStringAsync();

                // 2. Deserializamos la respuestaDTO ligera enviada por la API
                var datosCliente = JsonConvert.DeserializeObject<LoginResponseDto>(respuestaJson);

                // 3. Evaluamos si el usuario tiene el rol de Administrador
                bool esAdmin = datosCliente?.IdRols?.Any(r => r.Nombre == "ROLE_ADMIN") ?? false;

                // 4. Guardamos los datos en la sesión:
                //    a) Clave exclusiva para los datos del cliente (usado por el formulario de Contacto)
                HttpContext.Session.SetString("DatosClienteJson", respuestaJson);

                //    b) Clave para validar permisos de administrador en las vistas o filtros
                HttpContext.Session.SetString("EsAdmin", esAdmin ? "true" : "false");

                //    c) Clave histórica usada por otros módulos de tu equipo
                HttpContext.Session.SetString("UsuarioLogueado", correo);

                // REDIRECCIÓN SEGÚN EL ROL
                if (esAdmin) {
                    return RedirectToAction("ListaUsuarios", "Admin");
                }

                string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                var cliente_ = JsonConvert.DeserializeObject<ClienteDto>(contenidoRespuesta);

                HttpContext.Session.SetString("UsuarioLogueado", correo);
                HttpContext.Session.SetInt32("IdCliente", cliente_.IdCliente);

                return RedirectToAction("Inicio", "Producto");
            }
        }

        //Arreglo Test [Borrable]
        //private async Task CrearCookieClaimsDedicada(string jsonUsuario) {
        //    if (string.IsNullOrWhiteSpace(jsonUsuario)) return;

        //    // Parseo dinámico sin importar el DTO ni las mayúsculas
        //    using (var doc = System.Text.Json.JsonDocument.Parse(jsonUsuario)) {
        //        var root = doc.RootElement;

        //        string nombres = root.TryGetProperty("nombres", out var eN) ? eN.GetString() ?? "" :
        //                         root.TryGetProperty("Nombres", out var eN2) ? eN2.GetString() ?? "" : "";

        //        string apellidos = root.TryGetProperty("apellidos", out var eA) ? eA.GetString() ?? "" :
        //                           root.TryGetProperty("Apellidos", out var eA2) ? eA2.GetString() ?? "" : "";

        //        string correo = root.TryGetProperty("correo", out var eC) ? eC.GetString() ?? "" :
        //                        root.TryGetProperty("Correo", out var eC2) ? eC2.GetString() ?? "" : "";

        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, $"{nombres} {apellidos}".Trim()),
        //    new Claim(ClaimTypes.Email, correo)
        //};

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            new ClaimsPrincipal(claimsIdentity));
        //    }
        //}

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}