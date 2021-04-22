using System.ComponentModel.DataAnnotations;

namespace DoDoHack.ViewModels
{
    public class CreateNewsInput
    {
        [Required(ErrorMessage = "Вы не ввели заголовок.")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "Вы не ввели текст новости.")]
        public string Body { get; set; }
    }
}
