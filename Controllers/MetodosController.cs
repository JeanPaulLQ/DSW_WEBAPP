using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class MetodosController : Controller
    {
        private readonly IConfiguration _config;
        public MetodosController(IConfiguration config)
        {
            _config = config;
        }
        #region Métodos privados para consumir API
        private List<MetodosPagos> getMetodos()
        {
            var listado = new List<MetodosPagos>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Metodos").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<MetodosPagos>>(data);
            }
            return listado;
        }

        private MetodosPagos getMetodoById(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Metodos/{id}").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;

                if (!mensaje.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<MetodosPagos>(data);
            }
        }
        private bool updateMetodo(MetodosPagos metodo)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(metodo), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PutAsync("Metodos/update", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }

        private bool createMetodo(MetodosPagos metodo)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(metodo), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Metodos/register", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }

        private bool deleteMetodo(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.DeleteAsync($"Metodos/{id}").Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        #endregion
        public IActionResult Index()
        {
            var listado = getMetodos();
            return View(listado);
        }
        public IActionResult Create()
        {
            return View(new MetodosPagos());
        }

        [HttpPost]
        public IActionResult Create(MetodosPagos metodo)
        {
            createMetodo(metodo);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            deleteMetodo(id);
            return RedirectToAction("Index");
        }
        public IActionResult Edit(long id)
        {
            var metodo = getMetodoById(id);
            if (metodo == null) return NotFound();
            return View(metodo);
        }

        [HttpPost]
        public IActionResult Edit(MetodosPagos metodo)
        {
            updateMetodo(metodo);
            return RedirectToAction("Index");
        }

    }
}
