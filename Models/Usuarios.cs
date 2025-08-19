using System.ComponentModel.DataAnnotations;

namespace MusikWebApp.Models
{
    public class Usuarios
    {
        public long id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Registro")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fecha_registro { get; set; }
        public string direccion { get; set; }
        public string correo { get; set; }
        public string password { get; set; }
        public string n_documento { get; set; }
        public Roles roles { get; set; }
        public TiposDocumentos tipo_documento { get; set; }

    }
}
