using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class CourierChangeInput
    {
        [Required(ErrorMessage = "Вы не ввели имя.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Вы не ввели имя.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Вы не ввели телефон.")]
        [Phone(ErrorMessage = "Неверный тип данных.")]
        public string Phone { get; set; }

        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Неверный формат строки")]
        public decimal Rating { get; set; }
    }
}
