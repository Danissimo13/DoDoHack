using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; }

        private readonly DodoBase _dbContext;
        private readonly IHashService _hashService;

        public LoginModel(DodoBase db, IHashService hashService)
        {
            _dbContext = db;
            _hashService = hashService;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(nameof(Admin))) return LocalRedirect("~/Admin");
                else return LocalRedirect($"~/Courier/{User.FindFirst("Id")?.Value}");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var users = _dbContext.Set<User>();

            User user = await users.FirstOrDefaultAsync(u => u.Login == Input.Login);
            string passwordHash = _hashService.GetStringHash(Input.Password);
            if ((user == null) || (passwordHash != user.Password))
            {
                ModelState.AddModelError("Input.Login", "Неверный логин или пароль.");
                return Page();
            }

            if((user.Role == nameof(Courier)) && ((user as Courier).Accepted == false))
            {
                ModelState.AddModelError("Input.Login", "Компания пока не подтвердила ваш аккаунт.");
                return Page();
            }

            Claim[] claims = new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            if (user.Role == nameof(Admin)) return LocalRedirect("~/Admin");
            else return LocalRedirect($"~/Courier/{user.Id}");
        }
    }
}
