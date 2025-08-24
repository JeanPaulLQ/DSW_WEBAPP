using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;
        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }
        #region
        private Usuarios registerUsuario(Usuarios user)
        {
            Usuarios newUser = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(user),
                    System.Text.Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Usuario", contenido).Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                newUser = JsonConvert.DeserializeObject<Usuarios>(data);
            }
            return newUser;
        }
        private List<Usuarios> listarUsuarios()
        {
            var listado = new List<Usuarios>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Usuarios").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Usuarios>>(data);

            }
            return listado;
        }
        #endregion
        public IActionResult Index()
        {
            //vista del login
            return View();
        }
        [HttpPost]
        public IActionResult Login(string correo, string password)
        {
            Usuarios user = null;
            Console.WriteLine(correo);
            Console.WriteLine(password);
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);

                var contenido = new StringContent(
                    JsonConvert.SerializeObject(new { correo = correo, password = password }),
                    Encoding.UTF8,
                    "application/json"
                );

                var respuesta = clienteHttp.PostAsync("Usuarios/login", contenido).Result;

                if (respuesta.IsSuccessStatusCode)
                {
                    var data = respuesta.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<Usuarios>(data);

                    HttpContext.Session.SetString("UsuarioNombre", user.nombre);
                    HttpContext.Session.SetString("UsuarioId", user.id_usuario.ToString());
                    HttpContext.Session.SetString("UsuarioRol", user.id_rol.ToString());
                    TempData["Success"] = $"Bienvenido {user.nombre}";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Correo o contraseña incorrectos";
                }
            }

            return View("Index");
        }
        [HttpGet]
        public IActionResult Perfil()
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(idUsuario))
                return RedirectToAction("Index", "Usuario");

            Usuarios user = null;

            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var respuesta = clienteHttp.GetAsync($"Usuarios/{idUsuario}").Result;

                if (respuesta.IsSuccessStatusCode)
                {
                    var data = respuesta.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<Usuarios>(data);
                }
            }

            return View(user);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Has cerrado sesión correctamente.";
            return RedirectToAction("Index", "Usuario");
        }
        [HttpGet]
        public IActionResult ListaUsuarios()
        {
            var listado = listarUsuarios();
            return View(listado);
        }
    }
}
