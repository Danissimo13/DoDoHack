using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class CreateOrderInput
    {
        [Required(ErrorMessage = "Вы не ввели номер клиента.")]
        public string ClientPhone { get; set; }

        [Required(ErrorMessage = "Вы не ввели комментарий.")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Вы не ввели адресс.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Вы не ввели номер квартиры.")]
        public string Apartment { get; set; }

        [Required(ErrorMessage = "Вы не ввели номера продуктов.")]
        public string ProductsIds { get; set; }

        [Required(ErrorMessage = "Вы не ввели итоговую сумму.")]
        [DataType(DataType.Currency, ErrorMessage = "Неверный формат строки.")]
        public int TotalCost { get; set; }

        public long? WorkZoneId { get; set; }
    }
}
