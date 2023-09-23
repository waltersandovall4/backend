using System.ComponentModel.DataAnnotations;

namespace dw_backend.Models
{
    public class NotificacionesModel
    {
        [Required(ErrorMessage = "Nombre es requerido.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Descripción es requerido.")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "Categoria es requerida.")]
        public string categoria { get; set; }

#nullable enable
        public string? usuario { get; set; }
        public int? id { get; set; }
    }
}
