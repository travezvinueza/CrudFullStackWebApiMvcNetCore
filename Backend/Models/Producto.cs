using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Price { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Description { get; set; } = string.Empty;
        public string? Ruta { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}