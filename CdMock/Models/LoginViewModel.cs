using System.ComponentModel.DataAnnotations;

namespace CdMock.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username kiritish shart")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password kiritish shart")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
