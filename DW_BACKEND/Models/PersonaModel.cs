using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace dw_backend.Models
{
    public class PersonaModel
    {
#nullable enable

        public string? nombre1 { get; set; }
        public string? nombre2 { get; set; }
        public string? nombre3 { get; set; }
        public string? nombre4 { get; set; }
        public string? nombre5 { get; set; }
        public string? nombre6 { get; set; }
        public string? apellido1 { get; set; }
        public string? apellido2 { get; set; }
        public string? apellido3 { get; set; }
        public string? dpi { get; set; }        
        public int? genero { get; set; }        
        public string? email { get; set; }        
        public string? telefono { get; set; }
        public int? tipoPersona { get; set; }        
        public string? nit { get; set; }
        public IFormFile? foto { get; set; }
        public string? usuario { get; set; }        
        public int? id { get; set; }
    }
}
