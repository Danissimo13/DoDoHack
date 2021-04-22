using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class RegistrationInput
    {
        [Required(ErrorMessage = "Вы не ввели логин.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Вы не ввели имя.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Вы не ввели фамилию.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Вы не ввели телефон.")]
        [Phone(ErrorMessage = "Неверный формат данных.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Вы не ввели пароль.")]
        public string Password { get; set; }

    }
}
