using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Models
{
    public class AddServiceModel
    {
        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Введите название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Последний платеж обязателен")]
        [Display(Name = "Введите последний платеж")]
        [DataType(DataType.Date)]
        public DateTime LastPayment { get; set; }

        [Required(ErrorMessage = "Период обязателен")]
        [Display(Name = "Введите период в днях")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Некорректные данные")]
        public int Period { get; set; }

        [Required(ErrorMessage = "Цена обязателен")]
        [Display(Name = "Введите цену")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Некорректные данные")]
        public int Price { get; set; }
    }
}
