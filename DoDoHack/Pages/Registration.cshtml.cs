using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoDoHack.Data;
using DoDoHack.Services.Abstractions;
using DoDoHack.ViewModels;
using DoDoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DoDoHack.Pages
{
    public class RegistrationModel : PageModel
    {
        [BindProperty]
        public RegistrationInput Input { get; set; }

        private readonly DodoBase _dbContext;
        private readonly IHashService _hashService;

        public RegistrationModel(DodoBase db, IHashService hashService)
        {
            _dbContext = db;
            _hashService = hashService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var users = _dbContext.Set<User>();
            var courierStatistics = _dbContext.Set<CourierStatistic>();
            var courierOrdersVisisons = _dbContext.Set<CourierOrdersVision>();

            if(users.Any(u => u.Login == Input.Login))
            {
                ModelState.AddModelError("Input.Login", "Данный логин уже занят.");
                return Page();
            }

            CourierStatistic courierStatistic = new CourierStatistic()
            {
                ClosedShiftsCount = 0,
                OpenedLinesCount = 0,
                CanceledOrdersCount = 0,
                ClosedOrdersCount = 0,
            };
            CourierOrdersVision courierOrdersVision = new CourierOrdersVision()
            {
                FromCost = uint.MinValue,
                ToCost = uint.MaxValue,
            };
            Courier newCourier = new Courier()
            {
                Name = Input.Name,
                Surname = Input.Surname,
                Login = Input.Login,
                Password = _hashService.GetStringHash(Input.Password),
                Phone = Input.Phone,
                Statistic = courierStatistic,
                OrdersVision = courierOrdersVision,
                Role = nameof(Courier)
            };

            await courierStatistics.AddAsync(courierStatistic);
            await courierOrdersVisisons.AddAsync(courierOrdersVision);
            await users.AddAsync(newCourier);

            await _dbContext.SaveChangesAsync();

            return LocalRedirect("~/Login");
        }
    }
}
