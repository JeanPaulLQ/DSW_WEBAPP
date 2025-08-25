using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class MarcasController : Controller
    {
        private readonly IConfiguration _config;
        public MarcasController(IConfiguration config)
        {
            _config = config;
        }
        #region
        private List<Marcas> getMarcas()
        {
            var listado = new List<Marcas>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Marcas").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Marcas>>(data);

            }
            return listado;
        }
        private Marcas getMarcaById(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Marcas/{id}").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;

                if (!mensaje.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Marcas>(data);
            }
        }
        private bool createMarca(Marcas marca)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(marca), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Marcas/register", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        private bool updateMarca(Marcas marca)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(marca), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PutAsync("Marcas/update", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        private bool deleteMarca(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.DeleteAsync($"Marcas/{id}").Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        #endregion
        public IActionResult Index()
        {
            var listado = getMarcas();
            return View(listado);
        }
        public IActionResult Create()
        {
            return View(new Marcas());
        }
        [HttpPost]
        public IActionResult Create(Marcas marca)
        {
            createMarca(marca);
            return RedirectToAction("Index");
        }
        public IActionResult Edit(long id)
        {
            var marca = getMarcaById(id);
            if (marca == null) return NotFound();
            return View(marca);
        }
        [HttpPost]
        public IActionResult Edit(Marcas marca)
        {
            updateMarca(marca);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            deleteMarca(id);
            return RedirectToAction("Index");
        }
    }
}
