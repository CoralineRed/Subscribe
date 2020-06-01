using System.ComponentModel.DataAnnotations;

namespace ProjectInformatics.Models
{
    public class LoginModel
    {
        [Display(Name ="Введите логин")]
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }

        [Display(Name = "Введите пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
