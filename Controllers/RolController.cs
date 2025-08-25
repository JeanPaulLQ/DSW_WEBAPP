using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;

namespace MusikWebApp.Controllers
{
    public class RolController : Controller
    {
        private readonly IConfiguration _config;
        public RolController(IConfiguration config)
        {
            _config = config;
        }
        #region
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
        #endregion

        public IActionResult Index()
        {
            var listado = getRoles();
            return View(listado);
        }
    }
}
