using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Models;
using TrainingManagement.Repository;
using Microsoft.EntityFrameworkCore;

public class TrainingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public TrainingController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Overview()
    {
        var user = await _userManager.GetUserAsync(User);
        var userTrainings = await _context.UserTrainings
            .Include(ut => ut.Training)
            .Where(ut => ut.UserId == user.Id)
            .ToListAsync();

        return View(userTrainings);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var userTraining = await _context.UserTrainings
            .Include(ut => ut.Training)
            .FirstOrDefaultAsync(ut => ut.TrainingId == id && ut.UserId == user.Id);

        if (userTraining == null)
        {
            return NotFound();
        }

        return View(userTraining);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CompleteTraining(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var userTraining = await _context.UserTrainings
            .FirstOrDefaultAsync(ut => ut.TrainingId == id && ut.UserId == user.Id);

        if (userTraining == null)
        {
            return NotFound();
        }

        userTraining.IsCompleted = true;
        userTraining.CompletionDateTime = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminOverview()
    {
        var trainings = _context.Trainings.ToList();
        return View(trainings);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Training training)
    {
        if (ModelState.IsValid)
        {
            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminOverview));
        }
        return View(training);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TrainingDetails(int id)
    {
        var training = await _context.Trainings
            .Include(t => t.UserTrainings)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (training == null)
        {
            return NotFound();
        }

        return View(training);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTraining(int id)
    {
        var training = await _context.Trainings.FindAsync(id);
        if (training == null)
        {
            return NotFound();
        }

        var users = await _userManager.Users.ToListAsync();
        var viewModel = new AssignTrainingViewModel
        {
            TrainingId = id,
            TrainingTitle = training.Title,
            Users = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                IsAssigned = _context.UserTrainings.Any(ut => ut.UserId == u.Id && ut.TrainingId == id)
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTraining(AssignTrainingViewModel viewModel)
    {
        var training = await _context.Trainings.FindAsync(viewModel.TrainingId);
        if (training == null)
        {
            return NotFound();
        }

        foreach (var user in viewModel.Users)
        {
            var existingAssignment = await _context.UserTrainings
                .FirstOrDefaultAsync(ut => ut.UserId == user.Id && ut.TrainingId == viewModel.TrainingId);

            if (user.IsAssigned && existingAssignment == null)
            {
                _context.UserTrainings.Add(new UserTraining
                {
                    UserId = user.Id,
                    TrainingId = viewModel.TrainingId,
                    IsCompleted = false
                });
            }
            else if (!user.IsAssigned && existingAssignment != null)
            {
                _context.UserTrainings.Remove(existingAssignment);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(TrainingDetails), new { id = viewModel.TrainingId });
    }

}