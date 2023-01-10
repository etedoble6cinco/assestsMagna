using System.ComponentModel.DataAnnotations;

namespace AMS.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contrase♫a")]
        [Compare("Password", ErrorMessage = "La confirmacion de la constrase♫a y la constrase♫a no son iguales.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
