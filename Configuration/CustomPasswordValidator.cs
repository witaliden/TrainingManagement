using Microsoft.AspNetCore.Identity;
using TrainingManagement.Models;

namespace TrainingManagement.Configuration
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : User
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var errors = new List<IdentityError>();
            var options = user.UserPasswordOptions;

            // Sprawdzenie długości hasła
            if (password.Length < options.RequiredPasswordLength)
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequiredPasswordLength),
                    Description = $"Hasło musi mieć co najmniej {options.RequiredPasswordLength} znaków."
                });
            }

            // Weryfikacja cyfr
            if (options.RequireDigit && !password.Any(char.IsDigit))
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequireDigit),
                    Description = "Hasło musi zawierać co najmniej jedną cyfrę."
                });
            }

            // Weryfikacja małych liter
            if (options.RequireLowercase && !password.Any(char.IsLower))
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequireLowercase),
                    Description = "Hasło musi zawierać co najmniej jedną małą literę."
                });
            }

            // Weryfikacja wielkich liter
            if (options.RequireUppercase && !password.Any(char.IsUpper))
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequireUppercase),
                    Description = "Hasło musi zawierać co najmniej jedną wielką literę."
                });
            }

            // Weryfikacja znaków specjalnych
            if (options.RequireNonAlphanumeric && !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequireNonAlphanumeric),
                    Description = "Hasło musi zawierać co najmniej jeden znak specjalny."
                });
            }

            // Weryfikacja unikalnych znaków
            if (password.Distinct().Count() < options.RequiredUniqueChars)
            {
                errors.Add(new IdentityError
                {
                    Code = nameof(options.RequiredUniqueChars),
                    Description = $"Hasło musi zawierać co najmniej {options.RequiredUniqueChars} unikalnych znaków."
                });
            }

            if (errors.Any())
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }
    }
}
