using System.ComponentModel.DataAnnotations;

namespace dw_backend.Models
{
    public class TipoPersonaModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string descripcion { get; set; }

#nullable enable
        public string? usuario { get; set; }
        public int? id { get; set; }
    }
}
