using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IConfiguration _config;
        public CategoriaController(IConfiguration config)
        {
            _config = config;
        }
        #region
        private List<Categorias> getCategorias()
        {
            var listado = new List<Categorias>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Categoria").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Categorias>>(data);
            }
            return listado;
        }
        private Categorias getCategoriaById(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Categoria/{id}").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;

                if (!mensaje.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Categorias>(data);
            }
        }
        private bool createCategoria(Categorias cat)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(cat), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Categoria/register", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        private bool updateCategoria(Categorias cat)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(cat), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PutAsync("Categoria/update", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        private bool deleteCategoria(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.DeleteAsync($"Categoria/{id}").Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        #endregion
        public IActionResult Index()
        {
            var listado = getCategorias();
            return View(listado);
        }
        public IActionResult Create()
        {
            return View(new Categorias());
        }

        [HttpPost]
        public IActionResult Create(Categorias cat)
        {
            createCategoria(cat);
            return RedirectToAction("Index");
        }
        public IActionResult Edit(long id)
        {
            var cat = getCategoriaById(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public IActionResult Edit(Categorias cat)
        {
            updateCategoria(cat);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            deleteCategoria(id);
            return RedirectToAction("Index");
        }
    }
}
