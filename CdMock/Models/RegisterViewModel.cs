using System.ComponentModel.DataAnnotations;

namespace CdMock.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ism kiritish shart")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Familiya kiritish shart")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username kiritish shart")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email kiritish shart")]
        [EmailAddress(ErrorMessage = "Email formati noto'g'ri")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password kiritish shart")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password kamida 6 ta belgidan iborat bo'lishi kerak")]                                                            
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password tasdiqlash shart")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwordlar mos kelmadi")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shartlarni qabul qilish shart")]
        public bool AgreeToTerms { get; set; }
    }
}
