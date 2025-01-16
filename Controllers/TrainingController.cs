using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Controllers
{
    public class TrainingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var availableTrainings = _context.Trainings
                .Where(t => !t.UserTrainings.Any(ut => ut.UserId == userId))
                .ToList();
            return View(availableTrainings);
        }

        public IActionResult MyTrainings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var completedTrainings = _context.UserTrainings
                .Where(ut => ut.UserId == userId && ut.IsCompleted)
                .Select(ut => ut.Training)
                .ToList();
            return View(completedTrainings);
        }

        public IActionResult CompleteTraining(int trainingId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var userTraining = new UserTraining
            {
                UserId = userId.Value,
                TrainingId = trainingId,
                IsCompleted = true
            };
            _context.UserTrainings.Add(userTraining);
            _context.SaveChanges();

            return RedirectToAction("MyTrainings");
        }

        public IActionResult CreateTraining() {
            return View();
        }
    }
}
