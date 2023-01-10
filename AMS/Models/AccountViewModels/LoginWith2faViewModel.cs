using System.ComponentModel.DataAnnotations;

namespace AMS.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Codigo de Autenticacion")] //CODIGO DE AUTENTICACION PARA EL USUARIO PARA RESTABLECER EL ACCESO
        public string TwoFactorCode { get; set; }

        [Display(Name = "Confiar en este equipo")] // CHECKBOX  PARA QUE EL USUARIO NO VUELVA A HACER LOGIN  EN EL USUARIO SELECCIONADO   
        public bool RememberMachine { get; set; }
        // RECORDAR LA MAQUINA EN QUE EL USUARIO HIZO LOGIN 
        public bool RememberMe { get; set; }
        // RECORDAR LA SESSION CON LA QUE EL USUARIO HIZO LOGIN   

    }
}
