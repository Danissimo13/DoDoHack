using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class LoginInput
    {
        [Required(ErrorMessage = "Вы не ввели логин.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Вы не ввели пароль.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
