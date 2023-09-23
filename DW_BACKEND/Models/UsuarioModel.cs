using System.ComponentModel.DataAnnotations;

namespace dw_backend.Models
{
    public class UsuarioModel
    {
        [Required(ErrorMessage = "El id de la persona es requerido.")]
        public int persona { get; set; }

#nullable enable
        public string? roles { get; set; }
        public string? usuario { get; set; }
        public int? id { get; set; }
    }
}
