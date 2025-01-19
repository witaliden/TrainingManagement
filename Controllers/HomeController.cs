using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var viewModel = new DashboardViewModel
            {
                TotalTrainings = await _context.Trainings.CountAsync(),
                CompletedTrainingsCount = await _context.UserTrainings.CountAsync(ut => ut.IsCompleted),
                UpcomingTrainings = await _context.Trainings
                    .Where(t => t.DueDate > DateTime.Now)
                    .OrderBy(t => t.DueDate)
                    .Take(5)
                    .ToListAsync(),
                TopUsers = await _context.Users
                    .OrderByDescending(u => u.UserTrainings.Count(ut => ut.IsCompleted))
                    .Take(5)
                    .Select(u => new UserProgressViewModel
                    {
                        UserName = u.UserName,
                        CompletedTrainings = u.UserTrainings.Count(ut => ut.IsCompleted)
                    })
                    .ToListAsync()
            };

            if (currentUser != null)
            {
                viewModel.UserProgress = new UserProgressViewModel
                {
                    UserName = currentUser.UserName,
                    CompletedTrainings = await _context.UserTrainings.CountAsync(ut => ut.UserId == currentUser.Id && ut.IsCompleted),
                    TotalAssignedTrainings = await _context.UserTrainings.CountAsync(ut => ut.UserId == currentUser.Id)
                };
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
