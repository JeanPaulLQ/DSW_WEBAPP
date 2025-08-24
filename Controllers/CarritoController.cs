using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
namespace MusikWebApp.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IConfiguration _config;
        public CarritoController(IConfiguration config)
        {
            _config = config;
        }
        #region
        private List<MetodosPagos> listarMetodos()
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
        #endregion
        public IActionResult Index()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
            
            return View(carrito);
        }
        public IActionResult AgregarCarrito(long productoId,string nombre,decimal precio)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>():JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
            var item = carrito.FirstOrDefault(x => x.id_instrumentos == productoId);
            if (item != null) {
                item.cantidad++;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    id_instrumentos = productoId,
                    nombre = nombre,
                    precio = precio,
                    cantidad = 1
                });
            }
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
            return RedirectToAction("Index", "Carrito");

        }
        public IActionResult QuitarUnProducto(long id) {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
            var item = carrito.FirstOrDefault(x => x.id_instrumentos == id);
            if (item != null) {
                item.cantidad--;
                if(item.cantidad <= 0)
                {
                    carrito.Remove(item);
                }
            }
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
            return RedirectToAction("Index");
        }
        public IActionResult Eliminar(long id)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
            var item = carrito.FirstOrDefault(x => x.id_instrumentos == id);
            if (item != null)
                carrito.Remove(item);
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
            return RedirectToAction("Index");
        }
        public IActionResult Checkout() {
            var sesionIniciada = HttpContext.Session.GetString("UsuarioNombre");

            if (string.IsNullOrEmpty(sesionIniciada))
            {
                return RedirectToAction("Index","Usuario");
            }
            else
            {
                var carritoJson = HttpContext.Session.GetString("Carrito");
                var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
                var listMetodos = listarMetodos();
                ViewBag.MetodosPago = listMetodos;
                return View(carrito);
            }
            return RedirectToAction("Index");
        }
    }
}
