using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusikWebApp.Models
{
    public class Instrumentos
    {
        [DisplayName("ID")]
        public long id_instrumentos { get; set; }
        public string imagen { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public bool activo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Registro")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fecha_ingreso { get; set; }
        public long id_categoria { get; set; }
        public long id_marca { get; set; }
        [NotMapped]
        public string? nombreCategoria { get; set; }
        [NotMapped]
        public string? descripcionCategoria { get; set; }
        [NotMapped]
        public string? nombreMarca { get; set; }
    }
}
