using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingManagement.ActivityLogging;
using TrainingManagement.Models;
using TrainingManagement.Models.enums;
using TrainingManagement.Repository;

namespace TrainingManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserActivityLogger _activityLogger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserActivityLogger activityLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _activityLogger = activityLogger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                await _activityLogger.LogActivityAsync(User.Identity.Name, UserActionType.Login, true, "Pomyślne logowanie");
                var user = await _userManager.FindByNameAsync(username);
                if (user.PasswordExpirationDate.HasValue
                    && user.PasswordExpirationDate.Value < DateTime.UtcNow)
                {
                    return RedirectToAction("ChangeExpiredPassword", new { userId = user.Id });
                }
                else if (await _userManager.HasPasswordAsync(user) &&
                    !await _userManager.GetLockoutEnabledAsync(user))
                {
                    //TODO: dodać stronę z info o zablokowaniu konta
                    return RedirectToAction("ChangePassword", "Account");
                }
                return RedirectToAction("Index", "Home");
            }
            await _activityLogger.LogActivityAsync(username, UserActionType.Login, false, "Nieudana próba logowania");
            ModelState.AddModelError(string.Empty, "Login lub hasło niepoprawne");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    Lastname = model.Lastname,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    PasswordExpirationDate = DateTime.UtcNow.AddDays(90),
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

                var passwordValidator = new PasswordValidator<User>();
                var passwordValidationresult = await passwordValidator.ValidateAsync(_userManager, null, model.Password);
                if (!passwordValidationresult.Succeeded)
                {
                    foreach (var error in passwordValidationresult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                await _activityLogger.LogActivityAsync(user.UserName, UserActionType.RegisterUser, true, "Rejestracja nowego użytkownika");
                return RedirectToAction("Index", "Home");

            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Lastname = user.Lastname,
                PasswordExpirationDate = user.PasswordExpirationDate
            };

            ViewBag.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserProfile(ProfileViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Lastname = model.Lastname;
            user.PasswordExpirationDate = user.PasswordExpirationDate;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Dane użytkownika zostały zaktualizowane.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Profile", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new PasswordChangeViewModel
            {
                CurrentPassword = string.Empty,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var passwordValidator = new PasswordValidator<User>();
            var result = await passwordValidator.ValidateAsync(_userManager, null, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Profile", model);
            }

            user.LastPasswordChangedDate = DateTime.UtcNow;
            await _signInManager.RefreshSignInAsync(user);
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _activityLogger.LogActivityAsync(user.UserName, UserActionType.ChangePassword, true, "Zmiana hasła");
            TempData["StatusMessage"] = "Twoje hasło zostało zmienione.";

            return RedirectToAction(nameof(Profile));
        }


        [HttpPost]
        public async Task<IActionResult> ChangeExpiredPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Sprawdź czy nowe hasło nie było używane wcześniej
            foreach (var oldPassword in user.PreviousPasswords)
            {
                if (_userManager.PasswordHasher.VerifyHashedPassword(user, oldPassword, newPassword)
                    != PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(string.Empty, "Nie można użyć poprzedniego hasła.");
                    return View();
                }
            }

            // Zmień hasło i zaktualizuj daty
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                user.LastPasswordChangedDate = DateTime.UtcNow;
                user.PasswordExpirationDate = DateTime.UtcNow.AddDays(90); // domyślnie 90 dni
                user.PreviousPasswords.Add(_userManager.PasswordHasher.HashPassword(user, newPassword));
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity.Name;
            await _signInManager.SignOutAsync();
            await _activityLogger.LogActivityAsync(userName, UserActionType.Logout, true, "Wylogowanie z systemu");
            return RedirectToAction("Login");
        }
    }
}
