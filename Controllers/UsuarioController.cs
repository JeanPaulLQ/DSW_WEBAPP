using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private bool deleteUsuarios(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.DeleteAsync($"Usuarios/{id}").Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        private List<Roles> getRoles()
        {
            var listado = new List<Roles>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Rol").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Roles>>(data);
            }
            return listado;
        }
        private Usuarios getUsuarioPorId(long id)
        {
            Usuarios user = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var respuesta = clienteHttp.GetAsync($"Usuarios/{id}").Result;

                if (respuesta.IsSuccessStatusCode)
                {
                    var data = respuesta.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<Usuarios>(data);
                }
            }
            return user;
        }

        private bool updateUsuario(Usuarios usu)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(
                    JsonConvert.SerializeObject(usu),
                    Encoding.UTF8,
                    "application/json"
                );

                var respuesta = clienteHttp.PutAsync("Usuarios/update", contenido).Result;
                return respuesta.IsSuccessStatusCode;
            }
        }

        private List<TiposDocumentos> getDocumentos()
        {
            var listado = new List<TiposDocumentos>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("TipoDoc").Result;
                Console.WriteLine(mensaje);
                var data = mensaje.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);
                listado = JsonConvert.DeserializeObject<List<TiposDocumentos>>(data);
            }
            return listado;
        }
        private bool registerUsuario(Usuarios user)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Usuarios/register", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
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
        public IActionResult CreateUsuario()
        {
            var listaRoles = getRoles();
            var listaDoc = getDocumentos();
            ViewBag.Roles = new SelectList(listaRoles, "id_rol", "nombre");
            ViewBag.Documentos = new SelectList(listaDoc, "id_tipoDocumento", "nombre_tipo");
            return View(new Usuarios());
        }
        [HttpPost]
        public IActionResult CreateUsuario(Usuarios user)
        {
            registerUsuario(user);
            return RedirectToAction("ListaUsuarios");
        }
        [HttpPost]
        public IActionResult DeleteUsuario(long id)
        {
            bool result = deleteUsuarios(id);
            if (result)
            {
                TempData["Mensaje"] = "Usuario eliminado correctamente.";
                TempData["TipoMensaje"] = "success";
            }
            else
            {
                TempData["Mensaje"] = "No se pudo eliminar al usuario.";
                TempData["TipoMensaje"] = "danger";
            }
            return RedirectToAction("ListaUsuarios");
        }

        public IActionResult EditUsuario(long id)
        {
            var inst = getUsuarioPorId(id);
            var listaRoles = getRoles();
            var listaDoc = getDocumentos();
            ViewBag.Roles = new SelectList(listaRoles, "id_rol", "nombre");
            ViewBag.Documentos = new SelectList(listaDoc, "id_tipoDocumento", "nombre_tipo");
            
            if (inst == null) return NotFound();
            return View(inst);
        }

        [HttpPost]
        public IActionResult EditUsuario(Usuarios usu)
        {
            updateUsuario(usu);
            return RedirectToAction("ListaUsuarios");
        }
    }
}
