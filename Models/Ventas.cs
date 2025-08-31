namespace MusikWebApp.Models
{
    public class Ventas
    {
        public long id_venta { get; set; }
        public Usuarios usuario { get; set; }
        public MetodosPagos metodoPago { get; set; }
        public DateTime fecha_venta { get; set; }
        public decimal precioTotal { get; set; }
        public bool confirmada { get; set; }
    }
}