using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class CreateProductInput
    {
        [Required(ErrorMessage = "Вы не ввели название продукта.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Вы не ввели стоимость продукта.")]
        [DataType(DataType.Currency, ErrorMessage = "Неверный формат строки.")]
        public int Cost { get; set; }

        [Required(ErrorMessage = "Вы не загрузили изображение.")]
        public IFormFile Image { get; set; }
    }
}
