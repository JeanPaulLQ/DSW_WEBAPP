using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace MusikWebApp.Controllers
{
    public class InstrumentoController : Controller
    {
        private readonly IConfiguration _config;
        public InstrumentoController(IConfiguration config)
        {
            _config = config;
        }
        
        #region Private Methods
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

        private Instrumentos registrarInstrumento(Instrumentos instrumentos)
        {
            Instrumentos nuevoInstrumento = null;
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL"]);
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(instrumentos),
                    System.Text.Encoding.UTF8, "application/json");
                var mensaje = clienteHTTP.PostAsync("Instrumento/register", contenido).Result;
                
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    nuevoInstrumento = JsonConvert.DeserializeObject<Instrumentos>(data);
                }
            }
            return nuevoInstrumento;
        }

        private List<Categorias> getCategorias()
        {
            var listado = new List<Categorias>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Categoria").Result;
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    listado = JsonConvert.DeserializeObject<List<Categorias>>(data);
                }
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
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    listado = JsonConvert.DeserializeObject<List<Marcas>>(data);
                }
            }
            return listado;
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

        private IActionResult ReturnViewWithError(Instrumentos instrumentos, string error, List<Categorias> categorias = null, List<Marcas> marcas = null)
        {
            ViewBag.Categorias = new SelectList(categorias ?? getCategorias(), "id_categoria", "nombre");
            ViewBag.Marcas = new SelectList(marcas ?? getMarcas(), "id_marca", "nombre");
            ViewBag.Error = error;
            return View(instrumentos);
        }
        private Instrumentos updateInstrumento(Instrumentos instrumentos)
        {
            Instrumentos instrumentoActualizado = null;
            using (var clienteHTTP = new HttpClient())
            {
                clienteHTTP.BaseAddress = new Uri(_config["Services:URL"]);
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(instrumentos),
                    System.Text.Encoding.UTF8, "application/json");
                var mensaje = clienteHTTP.PutAsync($"Instrumento/{instrumentos.id_instrumentos}", contenido).Result;

                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    instrumentoActualizado = JsonConvert.DeserializeObject<Instrumentos>(data);
                }
            }
            return instrumentoActualizado;
        }
        #endregion

        public IActionResult Index()
        {
            var listado = getInstrumentos();
            return View(listado);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(getCategorias(), "id_categoria", "nombre");
            ViewBag.Marcas = new SelectList(getMarcas(), "id_marca", "nombre");
            return View(new Instrumentos());
        }
      
        [HttpPost]
        public IActionResult Create(Instrumentos instrumentos)
        {
            instrumentos.fecha_ingreso = DateTime.Now;
            instrumentos.activo = true;
       

            var categoriaId = Request.Form["categoria.id_categoria"];
            var marcaId = Request.Form["marca.id_marca"];
            var categorias = getCategorias();
            var marcas = getMarcas();
            
            // Asignar categoría y marca
            long.TryParse(categoriaId, out long catId);
            long.TryParse(marcaId, out long marId);
            instrumentos.categoria = categorias.FirstOrDefault(c => c.id_categoria == catId);
            instrumentos.marca = marcas.FirstOrDefault(m => m.id_marca == marId);

            // Registrar instrumento
            var resultado = registrarInstrumento(instrumentos);
            
            if (resultado != null)
            {
                TempData["SuccessMessage"] = $"El instrumento {instrumentos.nombre} se ha registrado correctamente";
                return RedirectToAction("ListarInst");
            }
            else
            {
                return ReturnViewWithError(instrumentos, "Error al guardar el instrumento", categorias, marcas);
            }
        }

        public IActionResult Details(int id)
        {
            Instrumentos inst = getInstrumentoPorId(id);
            return View(inst);
        }

        public IActionResult ListarInst(int id) 
        {
            var listado = getInstrumentos();
            return View(listado);
        }
        public IActionResult Delete(long id)
        {
            // Obtener el nombre del instrumento antes de eliminarlo
            var instrumento = getInstrumentoPorId(id);
            string nombreInstrumento = instrumento?.nombre ?? "Instrumento";
            
            var resultado = deleteInstrumento(id);
            
            if (resultado)
            {
                TempData["SuccessMessage"] = $"El instrumento <strong>{nombreInstrumento}</strong> ha sido eliminado correctamente";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error al eliminar el instrumento <strong>{nombreInstrumento}</strong>";
            }
            
            return RedirectToAction("ListarInst");
        }                
        public IActionResult Edit(long id)
        {
            var instrumento = getInstrumentoPorId(id);
            if (instrumento == null) return NotFound();
            
            ViewBag.Categorias = new SelectList(getCategorias(), "id_categoria", "nombre", instrumento.categoria?.id_categoria);
            ViewBag.Marcas = new SelectList(getMarcas(), "id_marca", "nombre", instrumento.marca?.id_marca);
            return View(instrumento);
        }

        [HttpPost]
        public IActionResult Edit(Instrumentos instrumentos)
        {
            var categoriaId = Request.Form["categoria.id_categoria"];
            var marcaId = Request.Form["marca.id_marca"];
            var categorias = getCategorias();
            var marcas = getMarcas();
            
            // Asignar categoría y marca
            long.TryParse(categoriaId, out long catId);
            long.TryParse(marcaId, out long marId);
            instrumentos.categoria = categorias.FirstOrDefault(c => c.id_categoria == catId);
            instrumentos.marca = marcas.FirstOrDefault(m => m.id_marca == marId);

            // Actualizar instrumento
            var resultado = updateInstrumento(instrumentos);
            
            if (resultado != null)
            {
                TempData["SuccessMessage"] = $"El instrumento {instrumentos.nombre} se ha actualizado correctamente";
                return RedirectToAction("ListarInst");
            }
            else
            {
                return ReturnViewWithError(instrumentos, "Error al actualizar el instrumento", categorias, marcas);
            }
        }
      
    }
}
