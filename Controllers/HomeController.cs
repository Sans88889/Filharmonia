using System.Diagnostics;
using Filharmonia.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filharmonia.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
        public IActionResult Repertuar()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Kup(string eventName, string date)
        {
            ViewData["EventName"] = eventName;
            ViewData["Date"] = date;
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmPurchase(string name, string email, string eventName, string date)
        {
            if (User.Identity.IsAuthenticated) // Jeœli u¿ytkownik jest zalogowany
            {
                name = User.Identity.Name; // Pobierz imiê u¿ytkownika (lub inne dane)
                email = "email@example.com"; // Mo¿esz pobraæ e-mail z bazy danych
            }

            // Tworzenie komunikatu z informacj¹ o zakupionym bilecie
            TempData["Message"] = $"Dziêkujemy, {name}! Zakupi³eœ bilet na \"{eventName}\" w dniu {date}.";
            return RedirectToAction("Index");
        }
    }
}
