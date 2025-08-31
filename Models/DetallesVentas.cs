namespace MusikWebApp.Models
{
    public class DetallesVentas
    {
        public long id_detalleVenta { get; set; }
        public Instrumentos instrumento { get; set; }
        public Ventas venta { get; set; }
        public int cantidad { get; set; }
        public decimal subTotal { get; set; }
    }
}