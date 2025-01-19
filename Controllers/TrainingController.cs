using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainings.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description,Link,DueDate")] Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            return View(training);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Link,DueDate")] Training training)
        {
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }

        /*[AllowAnonymous]
        public IActionResult Index()
        {
            if (!HttpContext)
                return RedirectToAction("Login", "Account");

            var userId = HttpContext.Session.GetString("UserId");
            var availableTrainings = _context.Trainings
                .Where(t => !t.UserTrainings.Any(ut => ut.UserId.Equals(userId)))
                .ToList();
            return View(availableTrainings);
        }

        public IActionResult MyTrainings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var completedTrainings = _context.UserTrainings
                .Where(ut => ut.UserId.Equals(userId) && ut.IsCompleted)
                .Select(ut => ut.Training)
                .ToList();
            return View(completedTrainings);
        }

        public IActionResult CompleteTraining(int trainingId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
                return RedirectToAction("Login", "Account");

            var userTraining = new UserTraining
            {
                UserId = userId != null ? userId : "anonymus",
                TrainingId = trainingId,
                IsCompleted = true
            };
            _context.UserTrainings.Add(userTraining);
            _context.SaveChanges();

            return RedirectToAction("MyTrainings");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateTraining() {
            return View();
        }*/
    }
}
