namespace MusikWebApp.Models
{
    public class CarritoItem
    {
        public long id_instrumentos { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; } = 1;
        public decimal subtotal => precio * cantidad; 
    }
}
