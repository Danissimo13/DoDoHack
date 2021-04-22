using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class MessagesModel : PageModel
    {
        public IEnumerable<Admin> DialogAdmins { get; set; }
        public IEnumerable<Courier> DialogCouriers { get; set; }

        private readonly DodoBase _dbContext;

        public MessagesModel(DodoBase db)
        {
            _dbContext = db;
        }

        public IActionResult OnGet()
        {
            var userId = long.Parse(User.FindFirst("Id").Value);

            var dialogWithUsersIds = _dbContext.Set<ChatMessage>()
                                               .Where(m => (m.ReceiverId == userId) || (m.SenderId == userId))
                                               .Select(m => m.ReceiverId == userId ? m.SenderId : m.ReceiverId)
                                               .Distinct();

            DialogAdmins = _dbContext.Set<Admin>()
                                     .Where(a => dialogWithUsersIds.Contains(a.Id));
            DialogCouriers = _dbContext.Set<Courier>()
                                       .Where(c => dialogWithUsersIds.Contains(c.Id));

            return Page();
        }
    }
}
