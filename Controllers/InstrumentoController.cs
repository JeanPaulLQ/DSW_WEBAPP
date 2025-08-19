using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;

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
    }
}
