using Microsoft.AspNetCore.Identity;
using TrainingManagement.Models;
using TrainingManagement.Repository;

namespace TrainingManagement.Configuration
{
    public class DataInitializerService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DataInitializerService(UserManager<User> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task InitializeData()
        {
            if (!_context.Users.Any())
            {
                await CreateUsers();
                await CreateTrainings();
            }
        }

        private async Task CreateUsers()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var admin = new User
            {
                UserName = "admin",
                Email = "admin@poczta.pl",
                Name = "Jestem",
                Lastname = "Główny",
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
            var createdAdmin = await _userManager.CreateAsync(admin, "Admin123!");

            if (createdAdmin.Succeeded)
                await _userManager.AddToRoleAsync(admin, "Admin");
            else
                throw new Exception($"Nie udało się utworzyć konto admina: {string.Join(", ", createdAdmin.Errors.Select(e => e.Description))}");

            for (int i = 1; i <= 10; i++)
            {
                var employee = new User
                {
                    UserName = $"employee{i}",
                    Email = $"employee{i}@poczta.pl",
                    Name = "Pracuję-Tu",
                    Lastname = $"{i} lat",
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
                var createdEmployee = await _userManager.CreateAsync(employee, "Employee123!");
                if (!createdEmployee.Succeeded)
                {
                    throw new Exception($"Nie udało się utworzyć pracownika {i}: {string.Join(", ", createdEmployee.Errors.Select(e => e.Description))}");
                }
            }
        }

        private async Task CreateTrainings()
        {
            for (int i = 1; i <= 20; i++)
            {
                var training = new Training
                {
                    Title = $"Training {i}",
                    Description = $"Description for Training {i}",
                    Link = $"https://example.com/training{i}",
                    DueDate = DateTime.Now.AddDays(30)
                };
                _context.Trainings.Add(training);
            }
            await _context.SaveChangesAsync();
        }
    }

}
