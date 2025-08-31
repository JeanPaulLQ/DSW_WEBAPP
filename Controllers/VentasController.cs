using Microsoft.AspNetCore.Mvc;
using MusikWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace MusikWebApp.Controllers
{
    public class VentasController : Controller
    {
        private readonly IConfiguration _config;
        public VentasController(IConfiguration config)
        {
            _config = config;
        }

        #region Private Methods
        private List<Ventas> getVentas()
        {
            var listado = new List<Ventas>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync("Ventas").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;
                listado = JsonConvert.DeserializeObject<List<Ventas>>(data);
            }
            return listado;
        }

        private Ventas getVentaById(long id)
        {
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Ventas/{id}").Result;
                var data = mensaje.Content.ReadAsStringAsync().Result;

                if (!mensaje.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<Ventas>(data);
            }
        }

        private List<Usuarios> getUsuarios()
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

        private List<MetodosPagos> getMetodosPago()
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

        private List<DetallesVentas> getDetallesVenta(long idVenta)
        {
            var listado = new List<DetallesVentas>();
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                var mensaje = clienteHttp.GetAsync($"Ventas/{idVenta}/detalles").Result;
                if (mensaje.IsSuccessStatusCode)
                {
                    var data = mensaje.Content.ReadAsStringAsync().Result;
                    listado = JsonConvert.DeserializeObject<List<DetallesVentas>>(data);
                }
            }
            return listado;
        }
        #endregion

        public IActionResult Index()
        {
            try
            {
                var ventas = getVentas();
                var usuarios = getUsuarios();
                var metodosPago = getMetodosPago();

                // Crear diccionarios para lookup rápido
                ViewBag.Usuarios = usuarios.ToDictionary(u => u.id_usuario, u => u.nombre);
                ViewBag.MetodosPago = metodosPago.ToDictionary(m => m.id_metodoPago, m => m.nombre);

                return View(ventas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las ventas: {ex.Message}";
                return View(new List<Ventas>());
            }
        }

        public IActionResult Details(long id)
        {
            try
            {
                var venta = getVentaById(id);
                if (venta == null) 
                    return NotFound();

                var detalles = getDetallesVenta(id);
                var usuarios = getUsuarios();
                var metodosPago = getMetodosPago();

                // Pasar datos adicionales a la vista
                ViewBag.Venta = venta;
                ViewBag.Usuarios = usuarios.ToDictionary(u => u.id_usuario, u => u.nombre);
                ViewBag.MetodosPago = metodosPago.ToDictionary(m => m.id_metodoPago, m => m.nombre);

                return View(detalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los detalles: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}