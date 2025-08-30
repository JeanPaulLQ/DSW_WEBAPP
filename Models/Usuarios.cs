using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Display(Name = "Contraseña")]
        public string password { get; set; }
        [Display(Name = "Numero de Documento")]
        public string n_documento { get; set; }
        [Display(Name = "Rol")]
        public long id_rol { get; set; }
        [Display(Name = "Tipo Documento")]
        public long id_tipo_documento { get; set; }
        [NotMapped]
        public string? nombreRol { get; set; }
        [NotMapped]
        public string? nombreTipoDocumento { get; set; }
    }
}
