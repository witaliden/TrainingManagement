using Microsoft.AspNetCore.Authorization;
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
        public static readonly string ErrorMessageKey = "ErrorMessage";
        public static readonly string SuccessMessageKey = "SuccessMessage";
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
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Lastname = model.Lastname,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    UserPasswordOptions = new UserPasswordOptions
                    {
                        RequiredPasswordLength = 8,
                        RequireDigit = true,
                        RequireLowercase = true,
                        RequireUppercase = true,
                        RequireNonAlphanumeric = true,
                        RequiredUniqueChars = 1
                    }
                };
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

            var viewModel = new UserConfigModel
            {
                Id = id,
                EditUserViewModel = new UserDetailsViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Lastname = user.Lastname,
                },
                ChangeUserPasswordViewModel = new ChangeUserPasswordViewModel(),
                UserSecutityViewModel = new UserSecutityViewModel
                {
                    IsLockedOut = await _userManager.IsLockedOutAsync(user),
                    UserPasswordOptions = new UserPasswordOptions
                    {
                        RequiredPasswordLength = user.UserPasswordOptions.RequiredPasswordLength,
                        RequireDigit = user.UserPasswordOptions.RequireDigit,
                        RequireLowercase = user.UserPasswordOptions.RequireLowercase,
                        RequireUppercase = user.UserPasswordOptions.RequireUppercase,
                        RequireNonAlphanumeric = user.UserPasswordOptions.RequireNonAlphanumeric,
                        RequiredUniqueChars = user.UserPasswordOptions.RequiredUniqueChars
                    },
                    PasswordExpirationDate = user.PasswordExpirationDate
                },
                UserTrainings = userTrainings,
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(UserConfigModel model)
        {
            if (model.EditUserViewModel == null)
            {
                ModelState.AddModelError(string.Empty, "EditUserViewModel cannot be null.");
                return View(model);
            }

            var userId = model.Id;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var userTrainings = await _context.UserTrainings
                    .Include(ut => ut.Training)
                    .Where(ut => ut.UserId == userId)
                    .OrderBy(ut => ut.IsCompleted)
                    .ThenBy(ut => ut.Training.DueDate)
                    .ToListAsync();

                var viewModel = new UserConfigModel
                {
                    Id = userId,
                    EditUserViewModel = new UserDetailsViewModel
                    {
                        UserName = model.EditUserViewModel.UserName,
                        Email = model.EditUserViewModel.Email,
                        Name = model.EditUserViewModel.Name,
                        Lastname = model.EditUserViewModel.Lastname,
                    },
                    ChangeUserPasswordViewModel = new ChangeUserPasswordViewModel(),
                    UserSecutityViewModel = new UserSecutityViewModel
                    {
                        IsLockedOut = await _userManager.IsLockedOutAsync(user),
                    },
                    UserTrainings = userTrainings
                };
                return View(viewModel);
            }

            user.Email = model.EditUserViewModel.Email;
            user.UserName = model.EditUserViewModel.UserName;
            user.Name = model.EditUserViewModel.Name;
            user.Lastname = model.EditUserViewModel.Lastname;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData[SuccessMessageKey] = "Dane użytkownika zostały zaktualizowane.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData[ErrorMessageKey] = "Wystąpił błąd podczas aktualizacji danych użytkownika.";
                return View(model);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserLock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName == User.Identity.Name)
            {
                TempData[ErrorMessageKey] = "Nie możesz zablokować własnego konta.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData[SuccessMessageKey] = "Konto zostało odblokowane.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData[SuccessMessageKey] = "Konto zostało zablokowane.";
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPasswordOptions(UserConfigModel model, int daysValid)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var passwordOptions = model.UserSecutityViewModel.UserPasswordOptions;

            // Aktualizacja wymagań hasła
            user.UserPasswordOptions.RequiredPasswordLength = passwordOptions.RequiredPasswordLength;
            user.UserPasswordOptions.RequireDigit = passwordOptions.RequireDigit;
            user.UserPasswordOptions.RequireLowercase = passwordOptions.RequireLowercase;
            user.UserPasswordOptions.RequireUppercase = passwordOptions.RequireUppercase;
            user.UserPasswordOptions.RequireNonAlphanumeric = passwordOptions.RequireNonAlphanumeric;
            user.UserPasswordOptions.RequiredUniqueChars = passwordOptions.RequiredUniqueChars;

            user.PasswordExpirationDate = DateTime.UtcNow.AddDays(daysValid);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Konfiguracja hasła użytkownika została zaktualizowana. " +
                    $"Hasło będzie ważne przez {daysValid} dni.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                TempData[SuccessMessageKey] = "Hasło zostało zmienione pomyślnie.";
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

            if (user.UserName == User.Identity.Name)
            {
                TempData[ErrorMessageKey] = "Nie możesz usunąć własnego konta.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData[SuccessMessageKey] = "Użytkownik został usunięty.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData[ErrorMessageKey] = "Wystąpił błąd podczas usuwania użytkownika.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

    }
}
