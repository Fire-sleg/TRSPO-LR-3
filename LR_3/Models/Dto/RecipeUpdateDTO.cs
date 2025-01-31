﻿using System.ComponentModel.DataAnnotations;

namespace LR_3.Models.Dto
{
    public class RecipeUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ImageURL { get; set; }
    }
}
