using System.ComponentModel.DataAnnotations;

namespace LR_3.Models.Dto
{
    public class ProductUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Kcal { get; set; }
    }
}
