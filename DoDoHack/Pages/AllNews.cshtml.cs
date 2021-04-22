using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoDoHack.Data;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AllNewsModel : PageModel
    {
        public const int NewsOnPage = 10;

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }

        [BindProperty]
        public CreateNewsInput Input { get; set; }

        public IEnumerable<News> ArrNews { get; set; }

        private readonly DodoBase _dbContext;

        public AllNewsModel(DodoBase db)
        {
            _dbContext = db;
        }

        public IActionResult OnGet()
        {
            if(!PageNumber.HasValue) PageNumber = 1;

            ArrNews = _dbContext.Set<News>()
                             .OrderByDescending(n => n.PublishDate)
                             .Skip((PageNumber.Value - 1) * NewsOnPage)
                             .Take(NewsOnPage);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return LocalRedirect("~/AllNews");
            if(!User.IsInRole(nameof(Admin))) return LocalRedirect("~/Denied");

            long adminId = long.Parse(User.FindFirst("Id").Value);

            var allNews = _dbContext.Set<News>();

            News news = new News()
            {
                Topic = Input.Topic,
                Body = Input.Body,
                AuthorId = adminId,
                PublishDate = DateTime.Now
            };

            await allNews.AddAsync(news);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect($"~/News/{news.Id}");
        }
    }
}
