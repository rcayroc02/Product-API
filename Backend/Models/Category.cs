using System.ComponentModel.DataAnnotations;

namespace TuProyecto.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        // Relación: una categoría tiene muchos productos
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}