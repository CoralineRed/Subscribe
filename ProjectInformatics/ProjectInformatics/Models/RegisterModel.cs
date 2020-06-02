using System.ComponentModel.DataAnnotations;

namespace ProjectInformatics.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Введите Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Введите пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пожалуйста, подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; }
    }
}
