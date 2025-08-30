using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class InstrumentoController : Controller
    {
        private readonly IConfiguration _config;
        public InstrumentoController(IConfiguration config)
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
        private List<Instrumentos> getInstrumentos()
        {
            var listado = new List<Instrumentos>();
            using(var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Instrumento").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Instrumentos>>(data);                

            }
            return listado;
        }
        private Instrumentos getInstrumentoPorId(long id)
        {
            Instrumentos inst = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Instrumento/{id}").Result;
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    inst = JsonConvert.DeserializeObject<Instrumentos>(data);
                }
            }
            return inst;
        }
        private bool createInstrumento(Instrumentos inst)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(inst), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PostAsync("Instrumento/register", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }

        private bool updateInstrumento(Instrumentos inst)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var contenido = new StringContent(JsonConvert.SerializeObject(inst), Encoding.UTF8, "application/json");
                var mensaje = clienteHttp.PutAsync("Instrumento/update", contenido).Result;
                return mensaje.IsSuccessStatusCode;
            }
        }

        private bool deleteInstrumento(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.DeleteAsync($"Instrumento/{id}").Result;
                return mensaje.IsSuccessStatusCode;
            }
        }
        #endregion
        public IActionResult Index()
        {
            var listado = getInstrumentos();
            return View(listado);
        }
        public IActionResult Details(int id)
        {
            Instrumentos inst = getInstrumentoPorId(id);
            return View(inst);
        }
        public IActionResult DetailsPanel(int id)
        {
            Instrumentos inst = getInstrumentoPorId(id);
            return View(inst);
        }
        public IActionResult ListarInst(int id) {
            var listado = getInstrumentos();
            return View(listado);
        }
        public IActionResult Create()
        {
            return View(new Instrumentos());
        }

        [HttpPost]
        public IActionResult Create(Instrumentos inst)
        {
            createInstrumento(inst);
            return RedirectToAction("ListarInst");
        }
        public IActionResult Edit(long id)
        {
            var listaMarcas = getMarcas();
            var listaCategorias = getCategorias();
            var inst = getInstrumentoPorId(id);
            ViewBag.listMarcas = new SelectList(listaMarcas, "id_marca", "nombre", inst.id_marca);
            ViewBag.listCategorias = new SelectList(listaCategorias, "id_categoria", "nombre", inst.id_categoria);
            if (inst == null) return NotFound();
            return View(inst);
        }

        [HttpPost]
        public IActionResult Edit(Instrumentos inst)
        {
            updateInstrumento(inst);
            return RedirectToAction("ListarInst");
        }
        [HttpPost]
        public IActionResult DeleteInst(long id)
        {
            bool result = deleteInstrumento(id);
            if (result)
            {
                TempData["Mensaje"] = "Instrumento eliminado correctamente.";
                TempData["TipoMensaje"] = "success";
            }
            else
            {
                TempData["Mensaje"] = "No se pudo eliminar el instrumento.";
                TempData["TipoMensaje"] = "danger";
            }
            return RedirectToAction("ListarInst");
        }
    }
}
