﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name, Lastname = model.Lastname };
                var result = await _userManager.CreateAsync(user, model.Password);
                result = await _userManager.SetLockoutEnabledAsync(user, false);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userTrainings = await _context.UserTrainings
                .Include(ut => ut.Training)
                .Where(ut => ut.UserId == id)
                .OrderBy(ut => ut.IsCompleted)
                .ThenBy(ut => ut.Training.DueDate)
                .ToListAsync();

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                UserTrainings = userTrainings
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AssignTrainings(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var allTrainings = await _context.Trainings.ToListAsync();
            var userTrainings = await _context.UserTrainings
                .Where(ut => ut.UserId == id)
                .Select(ut => ut.TrainingId)
                .ToListAsync();

            var viewModel = new AssignTrainingsViewModel
            {
                UserId = id,
                UserName = user.UserName,
                Trainings = allTrainings.Select(t => new TrainingAssignmentViewModel
                {
                    TrainingId = t.Id,
                    Title = t.Title,
                    IsAssigned = userTrainings.Contains(t.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTrainings(AssignTrainingsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentAssignments = await _context.UserTrainings
                .Where(ut => ut.UserId == model.UserId)
                .ToListAsync();

            foreach (var training in model.Trainings)
            {
                var existingAssignment = currentAssignments.FirstOrDefault(ca => ca.TrainingId == training.TrainingId);

                if (training.IsAssigned && existingAssignment == null)
                {
                    _context.UserTrainings.Add(new UserTraining
                    {
                        UserId = model.UserId,
                        TrainingId = training.TrainingId,
                        IsCompleted = false
                    });
                }
                else if (!training.IsAssigned && existingAssignment != null)
                {
                    _context.UserTrainings.Remove(existingAssignment);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = model.UserId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Generuj token resetowania hasła
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Resetuj hasło użytkownika
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Hasło zostało zmienione pomyślnie.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Nie pozwól administratorowi usunąć własnego konta
            if (user.UserName == User.Identity.Name)
            {
                TempData["ErrorMessage"] = "Nie możesz usunąć własnego konta.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Użytkownik został usunięty.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            TempData["ErrorMessage"] = "Wystąpił błąd podczas usuwania użytkownika.";
            return RedirectToAction(nameof(Index));
        }
    }
}
