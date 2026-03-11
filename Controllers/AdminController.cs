using Filharmonia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Filharmonia.Data;

namespace Filharmonia.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEventService _myEventService;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEventService myEventService)
        {
            _context = context;
            _userManager = userManager;
            _myEventService = myEventService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userRoles = await _userManager.GetRolesAsync(currentUser);
            Console.WriteLine($"Roles for user {currentUser.UserName}: {string.Join(", ", userRoles)}");

            return View();
        }

        public IActionResult UsersReport()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult ActivityReport()
        {
            var activityStats = new ActivityReportModel
            {
                Logins = 120,
                Registrations = 40,
                TicketsPurchased = 100
            };

            return View("ActivityReport", activityStats);
        }

        [HttpGet]
        [Route("Admin/TicketsReport")]
        public IActionResult TicketsReport()
        {
            var tickets = _myEventService.GetEventReport();
            return View(tickets);
        }
    }

    public class ActivityReportModel
    {
        public int Logins { get; set; }
        public int Registrations { get; set; }
        public int TicketsPurchased { get; set; }
    }
}
