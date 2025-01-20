using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.Models;
using TrainingManagement.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
}
