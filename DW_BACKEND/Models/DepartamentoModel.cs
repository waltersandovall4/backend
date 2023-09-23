using System.ComponentModel.DataAnnotations;

namespace dw_backend.Models
{
    public class DepartamentoModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El código es requerido.")]
        public string codigo { get; set; }

#nullable enable
        public string? usuario { get; set; }
        public int? id { get; set; }
    }
}
