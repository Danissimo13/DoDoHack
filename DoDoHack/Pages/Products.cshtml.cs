using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoDoHack.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = nameof(Admin))]
    public class ProductsModel : PageModel
    {
        public const int ProductsOnPage = 10;

        [BindProperty(SupportsGet = true)]
        public int? PageNumber { get; set; }

        [BindProperty]
        public CreateProductInput Input { get; set; }

        public IEnumerable<Product> Products { get; set; }

        private readonly DodoBase _dbContext;
        private readonly IFileService _fileService;

        public ProductsModel(DodoBase db, IFileService fileService)
        {
            _dbContext = db;
            _fileService = fileService;
        }

        public IActionResult OnGet()
        {
            if(!PageNumber.HasValue) PageNumber = 1;

            Products = _dbContext.Set<Product>()
                                 .Skip((PageNumber.Value - 1) * ProductsOnPage)
                                 .Take(ProductsOnPage);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return OnGet();
            if((Input.Image == null))
            {
                ModelState.AddModelError("Input.Image", "Вы не загрузили изображение");
                return LocalRedirect("~/Products");
            }

            string fileName = await _fileService.CreateFileAsync("img/products/", Input.Image.OpenReadStream());

            Product product = new Product()
            {
                Name = Input.Name,
                Cost = Input.Cost,
                ImageName = fileName
            };

            await _dbContext.Set<Product>().AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect("~/Products");
        }

        public async Task<IActionResult> OnPostDeleteAsync(long productId)
        {
            var products = _dbContext.Set<Product>();

            var product = await products.FirstOrDefaultAsync(p => p.Id == productId);
            if(product == null) return LocalRedirect("~/Products");

            products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return LocalRedirect("~/Products");
        }
    }
}
