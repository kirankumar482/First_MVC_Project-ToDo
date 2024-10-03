using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace First_MVC_Project.Models
{
    public class User
    { 
        [Required]
        public string? Name { get; set; }

        [Key]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [ValidateNever]
        public ICollection<Goal> Tasks { get; set; } = new List<Goal>();

    }
}