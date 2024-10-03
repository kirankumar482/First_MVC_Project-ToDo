using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace First_MVC_Project.Models
{
    public class Goal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(500)]
        [ValidateNever]
        public string? Description { get; set; }

        //Removed default value DateTime.Now here
        //It sets default value as 0/0/0001
        [Required]
        public DateTime CreatedDate { get; set; } 

        [Required]
        [FutureDate] //custom annotation
        public DateTime DueDate{ get; set; }

        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        //Navigation property
        [ValidateNever]
        public User? User { get; set; }
    }
    
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTimeValue)
            {
                if (dateTimeValue <= DateTime.Now)
                {
                    return new ValidationResult("The date must be in the future.");
                }
                return new ValidationResult("The date must be in the format dd/mm/yyyy");
            }
            return ValidationResult.Success;
        }
    }
}




//public DateTime DueDate
//{
//    get { return DueDate; } // Problem: getter refers to itself
//    set
//    {
//        if (value > DateTime.Now)
//        {
//            DueDate = value;  // Problem: setter assigns to itself
//        }
//        else
//        {
//            throw new ArgumentException("Date should be in the future.");
//        }
//    }
//}

//In this code:
//The getter calls DueDate instead of a backing field.
//The setter also assigns the value to DueDate, which calls the setter again, leading to an infinite loop.
//This causes a StackOverflowException because the setter is calling itself repeatedly without any exit condition.

//We have to use private fields in reading or writing data fields from Properties....