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
        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra(long id_metodoPago)
        {
            var sesionIniciada = HttpContext.Session.GetString("UsuarioNombre");
            var usuarioIdString = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(sesionIniciada) || string.IsNullOrEmpty(usuarioIdString))
            {
                TempData["Error"] = "Debes iniciar sesión para confirmar la compra.";
                return RedirectToAction("Index", "Usuario");
            }

            if (!long.TryParse(usuarioIdString, out long usuarioId))
            {
                TempData["Error"] = "Error en la sesión del usuario.";
                return RedirectToAction("Index", "Usuario");
            }

            // Obtener carrito de la sesión
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson) ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);

            if (!carrito.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            try
            {
                // Calcular total
                decimal total = carrito.Sum(x => x.subtotal) / 100;

                // Preparar datos para el stored procedure
                var carritoParaDB = carrito.Select(item => new
                {
                    id_instrumento = item.id_instrumentos,
                    cantidad = item.cantidad,
                    sub_total = (double)(item.subtotal / 100) // Convertir a double para que coincida con la BD
                }).ToList();

                var carritoJsonParaDB = JsonConvert.SerializeObject(carritoParaDB);

                // Preparar request para API
                var confirmarCompraRequest = new
                {
                    IdUsuario = usuarioId,
                    IdMetodoPago = id_metodoPago,
                    PrecioTotal = total,
                    CarritoJson = carritoJsonParaDB
                };

                // Llamar a la API para confirmar la compra
                using (var clienteHttp = new HttpClient())
                {
                    clienteHttp.BaseAddress = new Uri(_config["Services:URL"]);
                    var contenido = new StringContent(
                        JsonConvert.SerializeObject(confirmarCompraRequest),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var respuesta = await clienteHttp.PostAsync("Ventas/confirmar", contenido);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        // Limpiar carrito
                        HttpContext.Session.Remove("Carrito");
                        
                        TempData["Success"] = "¡Compra confirmada exitosamente! Gracias por tu compra.";
                        return RedirectToAction("CompraExitosa");
                    }
                    else
                    {
                        var errorContent = await respuesta.Content.ReadAsStringAsync();
                        var errorData = JsonConvert.DeserializeObject<dynamic>(errorContent);
                        
                        TempData["Error"] = $"Error al confirmar la compra: {errorData?.message ?? "Error desconocido"}";
                        return RedirectToAction("Checkout");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al procesar la compra: {ex.Message}";
                return RedirectToAction("Checkout");
            }
        }

        public IActionResult CompraExitosa()
        {
            return View();
        }
    }
}
