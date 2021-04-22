using DoDoHack.Data;
using DoDoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    public class ConversationModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Id { get; set; }

        public User DialogPartner { get; set; }

        private readonly DodoBase _dbContext;

        public ConversationModel(DodoBase db)
        {
            _dbContext = db;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            DialogPartner = await _dbContext.Set<User>().FirstOrDefaultAsync(a => a.Id == Id);
            if(DialogPartner == null) return LocalRedirect("~/Error/404");

            return Page();
        }
    }
}
